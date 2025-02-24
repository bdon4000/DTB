using DTB.Data.App.Status;

namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private object _deviceStateChartOption;
        private void UpdateDeviceStateChartOption()
        {
            var data = new List<(string Time, float[] States, float OEE)>();
            var standardPPM = deviceStatus?.DeviceInfo?.StandardPPM ?? 100;

            if (deviceStatus?.deviceChartDatas != null && deviceStatus.deviceChartDatas.Any())
            {
                var currentTime = ShiftStartTime;
                while (currentTime < ShiftEndTime)
                {
                    var nextTime = currentTime.Add(TimeInterval);

                    var periodData = deviceStatus.deviceChartDatas
                        .Where(d => d.StartTime >= currentTime && d.StartTime < nextTime)
                        .ToList();

                    var stateStats = new float[6];
                    foreach (var record in periodData)
                    {
                        for (int i = 0; i < record.StateStatistics.Length; i++)
                        {
                            stateStats[i] += record.StateStatistics[i] / 60f;
                        }
                    }

                    var runningTimeMinutes = stateStats[(int)DeviceState.Running];
                    var okProducts = periodData.Sum(x => x.OkOutput);
                    var theoreticalOutput = (runningTimeMinutes * standardPPM);
                    var oee = theoreticalOutput > 0 ? (okProducts / theoreticalOutput) * 100 : 0;

                    data.Add((
                        currentTime.ToString("HH:mm"),
                        stateStats,
                        oee
                    ));

                    currentTime = nextTime;
                }
            }

            // 用于创建一致的序列对象结构
            Func<string, string, string, string, object[], object, object> createSeries = (name, type, stack, barWidth, data, itemStyle) => new
            {
                Name = name,
                Type = type,
                Stack = stack,
                BarWidth = barWidth,
                Data = data,
                ItemStyle = itemStyle,
            };

            var baseSeriesList = Enum.GetValues(typeof(DeviceState))
                .Cast<DeviceState>()
                .Where(state => state != DeviceState.Switching)
                .Select(state => {
                    var statusInfo = state.GetStatusInfo();
                    return createSeries(
                        I18n.T(statusInfo.Name),
                        "bar",
                        "x",
                        "15",
                        data.Select(x => x.States[(int)state]).Cast<object>().ToArray(),
                        new { Color = $"#{statusInfo.Color.R:X2}{statusInfo.Color.G:X2}{statusInfo.Color.B:X2}" }
                    );
                })
                .ToList();

            // 添加OEE折线图系列
            baseSeriesList.Add(createSeries(
                "OEE",
                "line",
                null,
                "15",
                data.Select(x => x.OEE).Cast<object>().ToArray(),
                new { Color = "#1A73E8" }
            ));

            _deviceStateChartOption = new
            {

                Tooltip = new
                {
                    Trigger = "axis",
                    AxisPointer = new { Type = "shadow" }
                },
                Legend = new
                {
                    Data = baseSeriesList.Select(s => ((dynamic)s).Name).ToArray()
                },
                XAxis = new
                {
                    Data = data.Select(x => x.Time).ToArray(),
                    AxisLine = new { Show = false }
                },

                YAxis = new[]
                {
            new
            {
                Type = "value",
                Name = "Minutes",
                Position = "left",  // 添加与右轴相同的属性
                Max = TimeInterval.TotalMinutes,       // 添加与右轴相同的属性
                Show = true,
                axisLine = new
                    {
                        Show = false,
                        LineStyle = new
                        {
                            Color = "#485585"
                        }
                    },
                    axisTick = new
                    {
                        Show = false
                    },
                    splitLine = new
                    {
                        Show = false
                    }
            },
            new
            {
                Type = "value",
                Name = "OEE %",
                Position = "right",
                Max = 100.0,
                Show = true,
                axisLine = new
                    {
                        Show = false,
                        LineStyle = new
                        {
                            Color = "#485585"
                        }
                    },
                    axisTick = new
                    {
                        Show = false
                    },
                    splitLine = new
                    {
                        Show = false
                    }
            }
        },
                Series = baseSeriesList.Select((s, index) =>
                {
                    dynamic series = s;
                    if (series.Name == "OEE")
                    {
                        return new
                        {
                            series.Name,
                            series.Type,
                            series.Stack,
                            series.BarWidth,
                            series.Data,
                            series.ItemStyle,
                            YAxisIndex = 1,
                            Symbol = "circle",
                            SymbolSize = 6,
                            Smooth = true,
                            LineStyle = new { Width = 2 }
                        };
                    }
                    return s;
                }).ToArray(),
                Grid = new { y2 = 25 }
            };
        }

    }
}
