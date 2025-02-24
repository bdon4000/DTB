using System.Drawing;

namespace DTB.Data.App.Status
{
    public class DeviceStatus
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public string ImagePath { get; set; }

        public DeviceStatus(string name, Color color, string imagePath)
        {
            Name = name;
            Color = color;
            ImagePath = imagePath;
        }
    }

    public enum DeviceState
    {
        Idle = 0,
        Running,
        Error,
        Maintenance,
        Offline,
        Switching
    }
    public static class DeviceStateExtensions
    {
        public static DeviceStatus GetStatusInfo(this DeviceState state)
        {
            switch (state)
            {
                case DeviceState.Idle:
                    return new DeviceStatus("Idle", ColorTranslator.FromHtml("#008FFB"), "/img/status/IDLE.svg");
                case DeviceState.Running:
                    return new DeviceStatus("Running", ColorTranslator.FromHtml("#05CD99"), "/img/status/RUNNING.svg");
                case DeviceState.Error:
                    return new DeviceStatus("Error", ColorTranslator.FromHtml("#FF4560"), "/img/status/ERROR.svg");
                case DeviceState.Maintenance:
                    return new DeviceStatus("Maintenance", ColorTranslator.FromHtml("#FEB019"), "/img/status/MAINTENANCE.svg");
                case DeviceState.Offline:
                    return new DeviceStatus("Offline", Color.Gray, "/img/status/OFFLINE.svg");
                case DeviceState.Switching:
                    return new DeviceStatus("Switching", Color.DarkOrange, "/img/status/MAINTENANCE.svg");
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
