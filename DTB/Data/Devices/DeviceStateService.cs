using DTB.Controllers;
using DTB.Controllers.HeartbeatController;
using DTB.Data.BatteryData.BaseModel;
using DTB.Hubs;
using DTB.Service;
using DTB.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DTB.Data.Devices
{
    public interface IDeviceStateService
    {
        Task<DeviceStatusClass> UpdateDeviceState(string deviceCode, DeviceStateModel model);
        Task<DeviceStatusClass> UpdateHeartbeat(string deviceCode, HeartBeatModel model);
        Task UpdateBatteryData(string deviceCode, List<FullBaseModel> batteryData);
        Task ClearDeviceDataForShiftChange(string? newShift);
    }

    public partial class DeviceStateService : IDeviceStateService, IDisposable
    {
        private readonly IHubContext<DeviceHub> _hubContext;
        private readonly DeviceStatusCacheFactory _cacheFactory;
        private readonly IDbContextFactory<BatteryDbContext> _contextFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly HeartbeatBackgroundService _heartbeatService;
        private readonly IShiftService _shiftService;
        private bool _disposed;

        public DeviceStateService(
            IHubContext<DeviceHub> hubContext,
            DeviceStatusCacheFactory cacheFactory,
            IDbContextFactory<BatteryDbContext> contextFactory,
            UserManager<AppUser> userManager,
            HeartbeatBackgroundService heartbeatService,
            IShiftService shiftService)
        {
            _hubContext = hubContext;
            _cacheFactory = cacheFactory;
            _contextFactory = contextFactory;
            _userManager = userManager;
            _heartbeatService = heartbeatService;
            _shiftService = shiftService;
        }

        public async Task<DeviceStatusClass> UpdateDeviceState(string deviceCode, DeviceStateModel model)
        {
            var device = await GetDeviceByCode(deviceCode);
            if (device == null)
                throw new DeviceNotFoundException(deviceCode);

            var cache = _cacheFactory.GetCache(device.Id);
            var currentStatus = cache.GetStatus();
            
            if (currentStatus.LastStateChangeTime == default)
            {
                currentStatus.LastStateChangeTime = DateTime.Now;
            }

            var newState = GetStateFromString(model.DeviceState);
            HandleStateChange(currentStatus, newState);

            currentStatus.Status = newState;
            currentStatus.StatusMsg = model.DeviceState;
            currentStatus.UpdateTime = model.DateTime ?? DateTime.Now;
            currentStatus.Operator = model.Operator;
            currentStatus.ErrorMsg = model.ErrorMessage != null
                ? string.Join(", ", model.ErrorMessage)
                : string.Empty;

            InitializeStateTimer(deviceCode);


            CalculateRealtimePPM(currentStatus);
            cache.UpdateStatus(currentStatus);
            await NotifyStateChange(deviceCode, currentStatus);

            return currentStatus;
        }

        public async Task<DeviceStatusClass> UpdateHeartbeat(string deviceCode, HeartBeatModel model)
        {
            var device = await GetDeviceByCode(deviceCode);
            if (device == null)
                throw new DeviceNotFoundException(deviceCode);

            _heartbeatService.UpdateHeartbeat(deviceCode);

            var cache = _cacheFactory.GetCache(device.Id);
            var currentStatus = cache.GetStatus();

            currentStatus.UpdateTime = model.DateTime ?? DateTime.Now;
            currentStatus.Operator = model.Operator;
            currentStatus.DeviceInfo = device;

            if (model.ElectricMeter.HasValue)
            {
                currentStatus.chartdatas ??= new List<float>();
                currentStatus.chartdatas.Add(model.ElectricMeter.Value);
                if (currentStatus.chartdatas.Count > 24)
                {
                    currentStatus.chartdatas.RemoveAt(0);
                }

                HandleElectricMeterUpdate(currentStatus, model.ElectricMeter.Value);
            }

            cache.UpdateStatus(currentStatus);
            await NotifyStateChange(deviceCode, currentStatus);

            return currentStatus;
        }

        public async Task UpdateBatteryData(string deviceCode, List<FullBaseModel> batteryData)
        {
            var device = await GetDeviceByCode(deviceCode);
            if (device == null)
                throw new DeviceNotFoundException(deviceCode);

            var cache = _cacheFactory.GetCache(device.Id);
            var currentStatus = cache.GetStatus();

            currentStatus.BatteryDataBuff ??= new List<FullBaseModel>();

            foreach (var data in batteryData)
            {
                UpdateDeviceChartData(currentStatus, data);
            }

            const int maxBufferSize = 100;
            currentStatus.BatteryDataBuff.InsertRange(0, batteryData);
            if (currentStatus.BatteryDataBuff.Count > maxBufferSize)
            {
                currentStatus.BatteryDataBuff.RemoveRange(
                    maxBufferSize,
                    currentStatus.BatteryDataBuff.Count - maxBufferSize
                );
            }

            CalculateRealtimePPM(currentStatus);
            cache.UpdateStatus(currentStatus);
            await NotifyStateChange(deviceCode, currentStatus);
        }

        public async Task ClearDeviceDataForShiftChange(string? newShift)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var devices = await context.Devices.ToListAsync();

            foreach (var device in devices)
            {
                var cache = _cacheFactory.GetCache(device.Id);
                var currentStatus = cache.GetStatus();

                var clearedStatus = new DeviceStatusClass
                {
                    DeviceInfo = currentStatus.DeviceInfo,
                    UpdateTime = DateTime.Now,
                    Shift = newShift,
                    Operator = null,
                    Status = 0,
                    StatusMsg = "Shift Changed",
                    ErrorMsg = string.Empty,
                    chartdatas = new List<float>(),
                    PPM = 0,
                    LastElectricMeter = 0,  // 重置电表读数
                    BatteryDataBuff = new List<FullBaseModel>(),
                    deviceChartDatas = new List<DeviceChartData>(),
                    LastStateChangeTime = DateTime.Now,
                    LastStatusTransitionTime = DateTime.Now
                };

                cache.UpdateStatus(clearedStatus);

                if (currentStatus.DeviceInfo != null)
                {
                    await NotifyStateChange(currentStatus.DeviceInfo.DeviceCode, clearedStatus);
                }
            }
        }

        private Task<DeviceModel> GetDeviceByCode(string deviceCode)
        {
            var device = _cacheFactory.GetDeviceByCode(deviceCode);
            return Task.FromResult(device);
        }

        private async Task NotifyStateChange(string deviceCode, DeviceStatusClass status)
        {
            await _hubContext.Clients.Group(deviceCode)
                .SendAsync("ReceiveDeviceStatus", status);
        }

        private static int GetStateFromString(string state) => state switch
        {
            "Idle" => 0,
            "Running" => 1,
            "Error" => 2,
            "Maintenance" => 3,
            "Offline" => 4,
            "Switching" => 5,
            _ => 4
        };

        partial void CleanupTimers();
        partial void CleanupHeartbeatTimers();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void CalculateRealtimePPM(DeviceStatusClass status)
        {
            const float MIN_WINDOW_SECONDS = 10; // 最小时间窗口10秒
            const int SMOOTHING_WINDOW_SIZE = 5; // 平滑窗口大小
            const float SMOOTHING_FACTOR = 0.3f; // 平滑因子，值越小平滑效果越强

            // 1. 检查设备状态
            if (status.Status != 1) // 1 表示运行状态
            {
                status.PPM = 0;
                status.SmoothedPPM = 0;
                status.PPMHistory.Clear();
                return;
            }

            if (status.BatteryDataBuff == null || !status.BatteryDataBuff.Any())
            {
                status.PPM = 0;
                status.SmoothedPPM = 0;
                return;
            }

            // 2. 确定计算起始时间
            var currentTime = DateTime.Now;
            var earliestDataTime = status.BatteryDataBuff.Min(x => x.uploadTime);
            var startTime = status.LastStatusTransitionTime;

            if (earliestDataTime > startTime)
            {
                startTime = earliestDataTime;
            }

            // 3. 应用时间窗口限制
            var windowStartTime = currentTime.AddMinutes(-2);
            if (windowStartTime > startTime)
            {
                startTime = windowStartTime;
            }

            // 4. 检查最小时间窗口
            var timeDiff = (currentTime - startTime).TotalSeconds;
            if (timeDiff < MIN_WINDOW_SECONDS)
            {
                // 时间太短，保持上一次的PPM值
                return;
            }

            // 5. 计算当前PPM
            var outputCount = status.BatteryDataBuff
                .Count(x => x.uploadTime >= startTime);

            var currentPPM = (float)(outputCount / (timeDiff / 60.0)); // 转换为每分钟

            // 6. 应用平滑处理
            status.PPM = currentPPM;
            status.PPMHistory.Enqueue(currentPPM);

            // 保持历史队列大小
            while (status.PPMHistory.Count > SMOOTHING_WINDOW_SIZE)
            {
                status.PPMHistory.Dequeue();
            }

            if (status.PPMHistory.Count > 0)
            {
                // 指数移动平均
                if (status.SmoothedPPM == 0)
                {
                    // 第一次计算，直接使用当前值
                    status.SmoothedPPM = currentPPM;
                }
                else
                {
                    // EMA = α * 当前值 + (1-α) * 上一次EMA
                    status.SmoothedPPM = SMOOTHING_FACTOR * currentPPM +
                                        (1 - SMOOTHING_FACTOR) * status.SmoothedPPM;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CleanupTimers();
                    CleanupHeartbeatTimers();
                }
                _disposed = true;
            }
        }
    }
}