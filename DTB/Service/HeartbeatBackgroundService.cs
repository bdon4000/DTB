using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using DTB.Data;
using DTB.Controllers.HeartbeatController;
using DTB.Data.Devices;
using DTB.Controllers;

namespace DTB.Services
{
    public class HeartbeatBackgroundService : BackgroundService
    {
        private readonly ConcurrentDictionary<string, DateTime> _lastHeartbeatTimes = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextFactory<BatteryDbContext> _contextFactory;
        private Timer? _timer;
        private const int HeartbeatTimeoutSeconds = 60; // 1分钟超时

        public HeartbeatBackgroundService(
            IServiceProvider serviceProvider,
            IDbContextFactory<BatteryDbContext> contextFactory)
        {
            _serviceProvider = serviceProvider;
            _contextFactory = contextFactory;
        }

        public void UpdateHeartbeat(string deviceCode)
        {
            _lastHeartbeatTimes[deviceCode] = DateTime.Now;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 初始化所有设备的心跳时间
            await using var context = await _contextFactory.CreateDbContextAsync(stoppingToken);
            var devices = await context.Devices.ToListAsync(stoppingToken);
            foreach (var device in devices)
            {
                _lastHeartbeatTimes[device.DeviceCode] = DateTime.Now;
            }

            _timer = new Timer(CheckHeartbeats, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            await Task.CompletedTask;
        }

        private async void CheckHeartbeats(object? state)
        {
            using var scope = _serviceProvider.CreateScope();
            var deviceStateService = scope.ServiceProvider.GetRequiredService<IDeviceStateService>();

            foreach (var (deviceCode, lastHeartbeat) in _lastHeartbeatTimes)
            {
                var timeSinceLastHeartbeat = (DateTime.Now - lastHeartbeat).TotalSeconds;
                if (timeSinceLastHeartbeat > HeartbeatTimeoutSeconds)
                {
                    try
                    {
                        var offlineModel = new DeviceStateModel
                        {
                            DeviceState = "Offline",
                            DateTime = DateTime.Now,
                            ErrorMessage = new[] { "Device heartbeat timeout" }
                        };

                        await deviceStateService.UpdateDeviceState(deviceCode, offlineModel);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating offline status for device {deviceCode}: {ex.Message}");
                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_timer != null)
            {
                await _timer.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}