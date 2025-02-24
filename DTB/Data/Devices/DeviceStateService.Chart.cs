using DTB.Data.BatteryData.BaseModel;
using DTB.Services;

namespace DTB.Data.Devices
{
    public partial class DeviceStateService
    {
        private DateTime GetCurrentShiftStartTime()
        {
            var currentTime = DateTime.Now;
            var shifts = _shiftService.GetCachedShifts();

            if (shifts == null || !shifts.Any())
                return DateTime.Now;

            foreach (var shift in shifts)
            {
                var startTime = TimeSpan.Parse(shift.StartTime);
                var endTime = TimeSpan.Parse(shift.EndTime);
                var currentTimeOfDay = currentTime.TimeOfDay;

                if (endTime < startTime)
                {
                    if (currentTimeOfDay >= startTime || currentTimeOfDay < endTime)
                    {
                        var shiftStartTime = currentTime.Date.Add(startTime);
                        if (currentTimeOfDay < endTime)
                            shiftStartTime = shiftStartTime.AddDays(-1);
                        return shiftStartTime;
                    }
                }
                else if (currentTimeOfDay >= startTime && currentTimeOfDay < endTime)
                {
                    return currentTime.Date.Add(startTime);
                }
            }

            return DateTime.Now;
        }

        private DeviceChartData GetOrCreateCurrentInterval(DeviceStatusClass status)
        {
            var shiftStartTime = GetCurrentShiftStartTime();
            var currentTime = DateTime.Now;
            var intervalMinutes = 5;

            var minutesSinceShiftStart = (currentTime - shiftStartTime).TotalMinutes;
            var currentIntervalNumber = (int)(minutesSinceShiftStart / intervalMinutes);
            var intervalStartTime = shiftStartTime.AddMinutes(currentIntervalNumber * intervalMinutes);

            status.deviceChartDatas ??= new List<DeviceChartData>();
            var currentInterval = status.deviceChartDatas
                .FirstOrDefault(x => x.StartTime == intervalStartTime);

            if (currentInterval == null)
            {
                var lastInterval = status.deviceChartDatas
                    .OrderByDescending(x => x.StartTime)
                    .FirstOrDefault();

                if (lastInterval != null && status.LastStateChangeTime != default)
                {
                    lastInterval.StateStatistics[status.LastState] +=
                        (float)(intervalStartTime - status.LastStateChangeTime).TotalSeconds;
                }

                currentInterval = new DeviceChartData
                {
                    StartTime = intervalStartTime,
                    OkOutput = 0,
                    NgOutput = 0,
                    StateStatistics = new float[6],
                    NgStatistics = new List<int>(),
                    ElectricMeter = 0,
                    CPK = 0,
                    PPM = 0
                };

                // Ensure the new interval has enough capacity for current NG reasons
                currentInterval.EnsureNgStatisticsCapacity(status.NgReasonMap.Count);

                status.deviceChartDatas.Add(currentInterval);

                status.deviceChartDatas = status.deviceChartDatas
                    .Where(x => x.StartTime >= shiftStartTime)
                    .OrderBy(x => x.StartTime)
                    .ToList();
            }

            return currentInterval;
        }

        private void UpdateDeviceChartData(DeviceStatusClass status, FullBaseModel batteryData)
        {
            var currentInterval = GetOrCreateCurrentInterval(status);

            if (batteryData.result)
            {
                currentInterval.OkOutput++;
            }
            else
            {
                currentInterval.NgOutput++;

                // Process NG reason if available
                string ngReason = batteryData.ngReason?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(ngReason))
                {
                    // 如果无原因，使用 "Unknown" 类型
                    currentInterval.NgStatistics[0]++; // "Unknown" 总是在索引 0
                }
                else
                {
                    // Add new NG reason to the map if it doesn't exist
                    if (!status.NgReasonMap.ContainsKey(ngReason))
                    {
                        status.NgReasonMap[ngReason] = status.NgReasonMap.Count;

                        // Resize NgStatistics in all intervals
                        foreach (var interval in status.deviceChartDatas)
                        {
                            interval.EnsureNgStatisticsCapacity(status.NgReasonMap.Count);
                        }
                    }

                    // Increment the corresponding NG reason counter
                    int ngReasonIndex = status.NgReasonMap[ngReason];
                    currentInterval.NgStatistics[ngReasonIndex]++;
                }
            }
        }
    }
}