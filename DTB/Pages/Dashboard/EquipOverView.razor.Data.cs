using DTB.Data.BatteryData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using DTB.Data.Devices;

namespace DTB.Pages.Dashboard
{
    public partial class EquipOverView
    {
        private async Task CalculateProductionStatistics()
        {
            try
            {
                using var context = await DbContextFactory.CreateDbContextAsync();
                
                // Get current shift time range
                DateTime shiftStartTime = DateTime.Now.Date;
                DateTime shiftEndTime = DateTime.Now;
                
                if (currentShift != null && 
                    TimeSpan.TryParse(currentShift.StartTime, out var startTimeSpan) && 
                    TimeSpan.TryParse(currentShift.EndTime, out var endTimeSpan))
                {
                    var now = DateTime.Now;
                    shiftStartTime = now.Date.Add(startTimeSpan);
                    shiftEndTime = now.Date.Add(endTimeSpan);
                    
                    // Handle overnight shifts
                    if (endTimeSpan < startTimeSpan && now.TimeOfDay < endTimeSpan)
                    {
                        shiftStartTime = shiftStartTime.AddDays(-1);
                    }
                    else if (endTimeSpan < startTimeSpan && now.TimeOfDay >= startTimeSpan)
                    {
                        shiftEndTime = shiftEndTime.AddDays(1);
                    }
                }
                
                // Get JellyFeeding count
                totalJellyFeedingCount = await context.JellyFeedingDatas
                    .Where(j => j.uploadTime >= shiftStartTime && j.uploadTime <= shiftEndTime)
                    .CountAsync();
                
                // Get PreCharge count
                totalPreChargeCount = await context.PreChargeDatas
                    .Where(p => p.uploadTime >= shiftStartTime && p.uploadTime <= shiftEndTime)
                    .CountAsync();
                
                // Calculate total NG count from all devices
                totalNgCount = deviceStatuses
                    .SelectMany(s => s.deviceChartDatas ?? new List<DeviceChartData>())
                    .Sum(d => d.NgOutput);
                
                // Calculate yield rate
                if (totalJellyFeedingCount > 0)
                {
                    totalYieldRate = 100 * (1 - (float)totalNgCount / totalJellyFeedingCount);
                }
                else
                {
                    totalYieldRate = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating production statistics: {ex.Message}");
            }
        }

        private async Task InitializeShiftDataAsync()
        {
            try
            {
                await ShiftService.RefreshCache();
                isInitialized = true;
                
                var shifts = await ShiftService.GetCachedShiftsAsync();
                if (!shifts.Any())
                {
                    return;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing shift data: {ex.Message}");
            }
        }

        private async Task RefreshDeviceStatuses()
        {
            deviceStatuses.Clear();
            
            foreach (var device in allDevices)
            {
                var cache = CacheFactory.GetCache(device.Id);
                var status = cache.GetStatus();
                if (status != null)
                {
                    deviceStatuses.Add(status);
                }
            }
            
            await CalculateProductionStatistics();
        }
    }
}
