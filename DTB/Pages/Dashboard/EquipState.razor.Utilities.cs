using DTB.Data.Devices;
using DTB.Service;
using DTB.Services;

namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private Shift? currentShift;
        private DateTime RoundToHalfHour(DateTime dt, bool roundUp)
        {
            var minute = dt.Minute;
            var remainder = minute % 30;

            if (roundUp && remainder > 0)
            {
                return dt.AddMinutes(30 - remainder).AddSeconds(-dt.Second);
            }
            return dt.AddMinutes(-remainder).AddSeconds(-dt.Second);
        }



        private async Task<(DateTime start, DateTime end)> GetCurrentShiftTimeAsync()
        {
            try
            {
                if (!isInitialized)
                {
                    Console.WriteLine("ShiftService not yet initialized");
                    return GetDefaultShiftTime();
                }

                var shifts = await ShiftService.GetCachedShiftsAsync();
                Console.WriteLine();
                if (!shifts.Any())
                {
                    Console.WriteLine("No shifts found in cache");
                    return GetDefaultShiftTime();
                }

                var currentTime = DateTime.Now.TimeOfDay;
                 currentShift = shifts.FirstOrDefault(shift =>
                {
                    if (!TimeSpan.TryParse(shift.StartTime, out var startTime) ||
                        !TimeSpan.TryParse(shift.EndTime, out var endTime))
                    {
                        return false;
                    }
                    if (endTime < startTime)
                    {
                        return currentTime >= startTime || currentTime < endTime;
                    }
                    return currentTime >= startTime && currentTime < endTime;
                });

                if (currentShift == null)
                {
                    Console.WriteLine("No matching shift found for current time");
                    return GetDefaultShiftTime();
                }

                if (!TimeSpan.TryParse(currentShift.StartTime, out var shiftStart) ||
                    !TimeSpan.TryParse(currentShift.EndTime, out var shiftEnd))
                {
                    Console.WriteLine("Invalid shift time format");
                    return GetDefaultShiftTime();
                }

                var today = DateTime.Now.Date;
                DateTime startTime, endTime;

                if (shiftEnd < shiftStart) // 跨夜班次
                {
                    if (DateTime.Now.TimeOfDay < shiftEnd)
                    {
                        startTime = today.AddDays(-1).Add(shiftStart);
                        endTime = today.Add(shiftEnd);
                    }
                    else
                    {
                        startTime = today.Add(shiftStart);
                        endTime = today.AddDays(1).Add(shiftEnd);
                    }
                }
                else
                {
                    startTime = today.Add(shiftStart);
                    endTime = today.Add(shiftEnd);
                }

                return (startTime, endTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting shift time: {ex.Message}");
                return GetDefaultShiftTime();
            }
        }

        private async Task InitializeShiftDataAsync()
        {
            try
            {
                await ShiftService.RefreshCache();
                isInitialized = true;
                var (start, end) = await GetCurrentShiftTimeAsync();
                ShiftStartTime = RoundToHalfHour(start, false);
                ShiftEndTime = RoundToHalfHour(end, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing shift data: {ex.Message}");
                var defaultTimes = GetDefaultShiftTime();
                ShiftStartTime = defaultTimes.start;
                ShiftEndTime = defaultTimes.end;
            }
        }
        private (DateTime start, DateTime end) GetDefaultShiftTime()
        {
            var now = DateTime.Now;
            // 默认返回当前时间前8小时到当前时间的范围
            return (now.AddHours(-8), now);
        }
        
    }
}
