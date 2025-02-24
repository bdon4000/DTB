using System;

namespace DTB.Data.Devices
{
    public class DeviceNotFoundException : Exception
    {
        public string DeviceCode { get; }

        public DeviceNotFoundException(string deviceCode)
            : base($"Device with code '{deviceCode}' was not found.")
        {
            DeviceCode = deviceCode;
        }

        public DeviceNotFoundException(string deviceCode, string message)
            : base(message)
        {
            DeviceCode = deviceCode;
        }

        public DeviceNotFoundException(string deviceCode, string message, Exception innerException)
            : base(message, innerException)
        {
            DeviceCode = deviceCode;
        }
    }
}