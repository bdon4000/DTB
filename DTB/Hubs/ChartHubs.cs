using DTB.Data;
using DTB.Data.Devices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DTB.Hubs
{


    public class DeviceHub : Hub
    {
        private readonly DeviceStatusCacheFactory _cacheFactory;
        private readonly IDbContextFactory<BatteryDbContext> _contextFactory;

        public DeviceHub(
        DeviceStatusCacheFactory cacheFactory,
        IDbContextFactory<BatteryDbContext> contextFactory)
        {
            _cacheFactory = cacheFactory;
            _contextFactory = contextFactory;
        }
        // 加入设备分组
        public async Task JoinDeviceGroup(string deviceCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceCode);
        }

        // 离开设备分组
        public async Task LeaveDeviceGroup(string deviceCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, deviceCode);
        }

        // 发送图表数据到特定设备组
        public async Task SendDeviceState(string deviceCode, int state)
        {
            try
            {
                // 使用 DbContextFactory 创建新的上下文
                await using var context = await _contextFactory.CreateDbContextAsync();

                // 查询设备
                var device = await context.Devices
                    .FirstOrDefaultAsync(d => d.DeviceCode == deviceCode);

                if (device != null)
                {
                    var cache = _cacheFactory.GetCache(device.Id);
                    var status = cache.GetStatus();
                    await Clients.Group(deviceCode).SendAsync("ReceiveDeviceStatus", status);
                }
                else
                {
                    // 记录错误日志或处理设备未找到的情况
                    // 这里可以添加你的日志记录逻辑
                    Console.WriteLine($"Device not found: {deviceCode}");
                }
            }
            catch (Exception ex)
            {
                // 记录异常并处理错误
                // 这里可以添加你的错误处理逻辑
                Console.WriteLine($"Error in SendDeviceState: {ex.Message}");
                throw;
            }
        }
    }
}
