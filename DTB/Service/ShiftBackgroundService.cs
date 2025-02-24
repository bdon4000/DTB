using DTB.Data.Devices;
using DTB.Service;

namespace DTB.Services
{
    public class ShiftBackgroundService : BackgroundService
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer;

        public ShiftBackgroundService(
            
            IServiceScopeFactory scopeFactory)
        {
            
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(CheckShiftChange, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1)); // 每分钟检查一次

            return Task.CompletedTask;
        }

        private async void CheckShiftChange(object? state)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var shiftService = scope.ServiceProvider.GetRequiredService<IShiftService>();
                var shifts = shiftService.GetCachedShifts();

                var currentTime = DateTime.Now;
                foreach (var shift in shifts)
                {
                    var startTime = ParseTimeOfDay(shift.StartTime);
                    var endTime = ParseTimeOfDay(shift.EndTime);

                    if (IsMatchingTime(currentTime, startTime) || IsMatchingTime(currentTime, endTime))
                    {
                        var deviceStateService = scope.ServiceProvider.GetRequiredService<IDeviceStateService>();
                        string? newShift = IsMatchingTime(currentTime, startTime) ? shift.ShiftName : null;
                        await deviceStateService.ClearDeviceDataForShiftChange(newShift);
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error in shift change check: {ex}");
            }
        }

        private bool IsMatchingTime(DateTime current, TimeSpan target)
        {
            var currentTimeOfDay = current.TimeOfDay;
            return Math.Abs((currentTimeOfDay - target).TotalMinutes) < 1;
        }

        private TimeSpan ParseTimeOfDay(string timeString)
        {
            if (TimeSpan.TryParse(timeString, out TimeSpan result))
            {
                return result;
            }
            return TimeSpan.Zero;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }

    public class ShiftSetting
    {
        public string ShiftName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }
}