namespace DTB.Data.Devices
{
    public partial class DeviceStateService
    {
        private readonly Dictionary<string, System.Timers.Timer> _stateUpdateTimers = new();

        private void InitializeStateTimer(string deviceCode)
        {
            if (_stateUpdateTimers.ContainsKey(deviceCode))
            {
                return;
            }

            var timer = new System.Timers.Timer(30000);
            timer.Elapsed += async (sender, e) => await UpdateStateStatistics(deviceCode);
            timer.AutoReset = true;
            timer.Start();

            _stateUpdateTimers[deviceCode] = timer;
        }

        private async Task UpdateStateStatistics(string deviceCode)
        {
            var device = await GetDeviceByCode(deviceCode);
            if (device == null) return;

            var cache = _cacheFactory.GetCache(device.Id);
            var status = cache.GetStatus();

            UpdateCurrentStateTime(status);

            cache.UpdateStatus(status);
            await NotifyStateChange(deviceCode, status);
        }

        private void UpdateCurrentStateTime(DeviceStatusClass status)
        {
            if (status.LastStateChangeTime == default)
            {
                status.LastStateChangeTime = DateTime.Now;
                return;
            }

            var currentTime = DateTime.Now;
            var duration = (currentTime - status.LastStateChangeTime).TotalSeconds;

            var currentInterval = GetOrCreateCurrentInterval(status);
            if (currentInterval != null && status.LastState >= 0 && status.LastState < 6)
            {
                currentInterval.StateStatistics[status.LastState] += (float)duration;
            }

            status.LastStateChangeTime = currentTime;
        }

        private void HandleStateChange(DeviceStatusClass status, int newState)
        {
            UpdateCurrentStateTime(status);

            if (status.Status != newState)
            {
                status.LastStatusTransitionTime = DateTime.Now;
            }

            status.LastState = newState;
            status.LastStateChangeTime = DateTime.Now;

            var currentInterval = GetOrCreateCurrentInterval(status);
            if (currentInterval != null)
            {
                currentInterval.StateStatistics ??= new float[6];
            }
        }

        private void HandleElectricMeterUpdate(DeviceStatusClass status, float newElectricMeter)
        {
            if (status.LastElectricMeter == 0)
            {
                status.LastElectricMeter = newElectricMeter;
                return;
            }

            float electricDifference = newElectricMeter - status.LastElectricMeter;
            var currentInterval = GetOrCreateCurrentInterval(status);
            if (currentInterval != null)
            {
                currentInterval.ElectricMeter += electricDifference;
            }

            status.LastElectricMeter = newElectricMeter;
        }

        partial void CleanupTimers()
        {
            foreach (var timer in _stateUpdateTimers.Values)
            {
                timer.Stop();
                timer.Dispose();
            }
            _stateUpdateTimers.Clear();
        }
    }
}