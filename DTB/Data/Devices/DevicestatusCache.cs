namespace DTB.Data.Devices
{
    public interface IDeviceStatusCache
    {
        DeviceStatusClass GetStatus();
        void UpdateStatus(DeviceStatusClass status);
    }

    public class DeviceStatusCache : IDeviceStatusCache
    {
        private DeviceStatusClass _currentStatus;
        private readonly object _lock = new object();

        public DeviceStatusCache()
        {
            _currentStatus = new DeviceStatusClass
            {
                UpdateTime = DateTime.Now,
                Status = 0,
                StatusMsg = "Initialized",
                ErrorMsg = string.Empty
            };
        }

        public DeviceStatusClass GetStatus()
        {
            lock (_lock)
            {
                return _currentStatus;
            }
        }

        public void UpdateStatus(DeviceStatusClass status)
        {
            lock (_lock)
            {
                _currentStatus = status;
            }
        }
    }
}