namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private object _electricMeterChartOption;
        private void UpdateElectricMeterChartOption()
        {
            var data = new List<(string Time, float Value)>();
            //var accumulatedValues = new Dictionary<DateTime, float>();

            if (deviceStatus?.deviceChartDatas != null && deviceStatus.deviceChartDatas.Any())
            {
                // 首先计算每个时间点的累计值
                var currentTime = ShiftStartTime;
                while (currentTime <= ShiftEndTime)
                {
                    var nextTime = currentTime.Add(TimeInterval);

                    var periodData = deviceStatus.deviceChartDatas
                        .Where(d => d.StartTime >= currentTime && d.StartTime < nextTime)
                        .ToList();

                    // 计算到当前时间点的所有电能累计
                    var totalElectric = periodData.Sum(x => x.ElectricMeter);

                    data.Add((
                        currentTime.ToString("HH:mm"),
                        totalElectric
                    ));

                    currentTime = nextTime;
                }

                // 转换为图表数据

            }

            _electricMeterChartOption = new
            {
                Tooltip = new
                {
                    Trigger = "axis",
                    axisPointer = new
                    {
                        Type = "shadow"
                    }
                },
                XAxis = new
                {
                    Data = data.Select(x => x.Time).ToArray(),
                    axisLine = new
                    {
                        Show = false
                    },
                    axisLabel = new
                    {
                        Show = true,
                        Color = "#485585"
                    },
                    axisTick = new
                    {
                        Show = false
                    },
                    splitLine = new
                    {
                        Show = false
                    },
                },
                YAxis = new
                {
                    Type = "value",
                    Min = 0,
                    Max = data.Any() ? data.Max(x => x.Value) : 100,
                    Interval = data.Any() ? data.Max(x => x.Value) / 2 : 50,
                    axisLine = new
                    {
                        Show = false
                    },
                    axisLabel = new
                    {
                        Show = true,
                        Color = "#485585"
                    },
                    axisTick = new
                    {
                        Show = false
                    },
                    splitLine = new
                    {
                        Show = false
                    },
                },
                Series = new[]
    {
        new
        {
            Type = "bar",
            Data = data.Select(x => x.Value).ToArray(),
            Color = "#4318FF"
        }
    },
                Grid = new
                {
                    Left = "30",
                    Right = "10",
                    Top = "10",
                    Bottom = "20"
                }
            };
        }
    }
}
