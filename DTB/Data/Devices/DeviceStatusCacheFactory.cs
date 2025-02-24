using DTB.Data.BatteryData.BaseModel;

namespace DTB.Data.Devices
{
    public class DeviceStatusCacheFactory
    {
        private readonly Dictionary<int, IDeviceStatusCache> _caches;
        private readonly List<DeviceModel> _deviceModels;

        public DeviceStatusCacheFactory(List<DeviceModel> deviceModels)
        {
            _caches = new Dictionary<int, IDeviceStatusCache>();
            _deviceModels = deviceModels;
        }

        // 添加获取所有设备的方法
        public IEnumerable<DeviceModel> GetAllDevices()
        {
            return _deviceModels;
        }

        // 添加根据设备代码获取设备的方法
        public DeviceModel GetDeviceByCode(string deviceCode)
        {
            return _deviceModels.FirstOrDefault(d => d.DeviceCode == deviceCode);
        }

        public IDeviceStatusCache GetCache(int deviceId)
        {
            if (!_caches.ContainsKey(deviceId))
            {
                var deviceModel = _deviceModels.FirstOrDefault(d => d.Id == deviceId);
                var cache = new DeviceStatusCache();
                // 初始化设备状态时设置DeviceInfo
                cache.UpdateStatus(new DeviceStatusClass
                {
                    DeviceInfo = deviceModel,
                    UpdateTime = DateTime.Now,
                    Status = 0,
                    StatusMsg = "Initialized",
                    ErrorMsg = string.Empty,
                    chartdatas = new List<float>(),
                    BatteryDataBuff = new List<FullBaseModel>()
                });
                _caches[deviceId] = cache;
            }
            return _caches[deviceId];
        }

        // 可选：添加更新设备信息的方法
        public void UpdateDeviceInfo(DeviceModel updatedDevice)
        {
            var index = _deviceModels.FindIndex(d => d.Id == updatedDevice.Id);
            if (index != -1)
            {
                _deviceModels[index] = updatedDevice;
                // 如果该设备已经有缓存，更新缓存中的设备信息
                if (_caches.TryGetValue(updatedDevice.Id, out var cache))
                {
                    var currentStatus = cache.GetStatus();
                    currentStatus.DeviceInfo = updatedDevice;
                    cache.UpdateStatus(currentStatus);
                }
            }
        }
    }
}