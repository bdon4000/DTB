using DTB.Data.App.Status;

namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        // 添加OEE相关计算属性
        private float GetTotalTimeInMinutes()
        {
            if (deviceStatus?.deviceChartDatas == null || !deviceStatus.deviceChartDatas.Any())
                return 0;

            float totalMinutes = 0;
            foreach (var data in deviceStatus.deviceChartDatas)
            {
                // 累加所有状态的时间
                for (int i = 0; i < data.StateStatistics.Length; i++)
                {
                    totalMinutes += data.StateStatistics[i] / 60.0f; // 转换秒到分钟
                }
            }
            return totalMinutes;
        }

        private float CalculateAvailability()
        {
            if (deviceStatus?.deviceChartDatas == null || !deviceStatus.deviceChartDatas.Any())
                return 0;

            var totalTime = GetTotalTimeInMinutes();
            if (totalTime == 0) return 0;

            var runningTime = deviceStatus.deviceChartDatas.Sum(x => x.StateStatistics[(int)DeviceState.Running]) / 60.0;

            return (float)(runningTime / totalTime * 100);
        }

        private float CalculatePerformance()
        {
            if (deviceStatus?.deviceChartDatas == null || !deviceStatus.deviceChartDatas.Any())
                return 0;

            var runningTimeMinutes = deviceStatus.deviceChartDatas.Sum(x => x.StateStatistics[(int)DeviceState.Running]) / 60.0;
            if (runningTimeMinutes == 0)
                return 0;
            
            // 使用设备标准PPM计算理论产能
            var standardPPM = deviceStatus.DeviceInfo?.StandardPPM ?? 0;
            var standardOutput = runningTimeMinutes * standardPPM ; // 转换为每分钟的产能
            var actualOutput = TotalProduction;

            return (float)(actualOutput / standardOutput * 100);
        }

        private float CalculateQuality()
        {
            if (TotalProduction == 0)
                return 0;

            return (float)(TotalOkOutput) / TotalProduction * 100;
        }

        private float CalculateOEE()
        {
            var availability = CalculateAvailability();
            var performance = CalculatePerformance();
            var quality = CalculateQuality();

            return availability * performance * quality / 10000;
        }

        private string GetColorClass(float value)
        {
            if (value >= 90) return "success--text";
            if (value >= 70) return "warning--text";
            return "error--text";
        }
    }
}
