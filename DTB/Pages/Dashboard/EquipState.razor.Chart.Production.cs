namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private object _productionChartOption;
        private Dictionary<string, bool> selectedProduction = new()
        {
            { "OK", true },
            { "NG", true }
        };

        private void UpdateProductionChartOption()
        {
            var data = new List<(string Time, int OK, int NG)>();

            if (deviceStatus?.deviceChartDatas != null && deviceStatus.deviceChartDatas.Any())
            {
                var currentTime = ShiftStartTime;
                while (currentTime < ShiftEndTime)
                {
                    var nextTime = currentTime.Add(TimeInterval);

                    var periodData = deviceStatus.deviceChartDatas
                        .Where(d => d.StartTime >= currentTime && d.StartTime < nextTime)
                        .ToList();

                    var ok = periodData.Sum(x => x.OkOutput);
                    var ng = periodData.Sum(x => x.NgOutput);

                    data.Add((
                        currentTime.ToString("HH:mm"),
                        ok,
                        ng
                    ));

                    currentTime = nextTime;
                }
            }

            var series = new List<object>();

            // 只有当NG被选中时添加NG数据
            if (selectedProduction["NG"])
            {
                series.Add(new
                {
                    Name = "NG",
                    Type = "bar",
                    Data = data.Select(x => x.NG).ToArray(),
                    Color = "#FF4560",
                    Stack = "x",
                    BarWidth = "15"
                });
            }

            // 只有当OK被选中时添加OK数据
            if (selectedProduction["OK"])
            {
                series.Add(new
                {
                    Name = "OK",
                    Type = "bar",
                    Data = data.Select(x => x.OK).ToArray(),
                    Color = "#05CD99",
                    Stack = "x",
                    BarWidth = "15"
                });
            }

            _productionChartOption = new
            {
                Title = new
                {
                    Text = I18n.T("ProductionStatistics"),
                    TextStyle = new
                    {
                        Color = "#1B2559"
                    }
                },
                Tooltip = new
                {
                    Trigger = "axis",
                    axisPointer = new
                    {
                        Type = "shadow"
                    }
                },
                Legend = new
                {
                    Data = new[] { "OK", "NG" },
                    Right = "5px",
                    Selected = selectedProduction,
                    TextStyle = new
                    {
                        Color = "#485585",
                    }
                },
                XAxis = new
                {
                    Data = data.Select(x => x.Time).ToArray(),
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
                    },
                    TextStyle = new
                    {
                        Color = "#485585"
                    }
                },
                YAxis = new
                {
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
                Series = series,
                Grid = new
                {
                    y2 = 25
                }
            };
        }

        private object GetProductionChartData()
        {
            var data = new List<(string Time, int OK, int NG)>();

            if (deviceStatus?.deviceChartDatas != null && deviceStatus.deviceChartDatas.Any())
            {
                var currentTime = ShiftStartTime;
                while (currentTime < ShiftEndTime)
                {
                    var nextTime = currentTime.Add(TimeInterval);

                    // 获取这个时间段的数据
                    var periodData = deviceStatus.deviceChartDatas
                        .Where(d => d.StartTime >= currentTime && d.StartTime < nextTime)
                        .ToList();

                    var ok = periodData.Sum(x => x.OkOutput);
                    var ng = periodData.Sum(x => x.NgOutput);

                    data.Add((
                        currentTime.ToString("HH:mm"),
                        ok,
                        ng
                    ));

                    currentTime = nextTime;
                }
            }

            return new
            {
                Title = new
                {
                    Text = "Production Statistics",
                    TextStyle = new
                    {
                        Color = "#1B2559"
                    }
                },
                Tooltip = new
                {
                    Trigger = "axis",
                    axisPointer = new
                    {
                        Type = "shadow"
                    }
                },
                Legend = new
                {
                    Data = new[] { "OK", "NG" },
                    Right = "5px",
                    TextStyle = new
                    {
                        Color = "#485585",
                    }
                },
                XAxis = new
                {
                    Data = data.Select(x => x.Time).ToArray(),
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
                    },
                    TextStyle = new
                    {
                        Color = "#485585"
                    }
                },
                YAxis = new
                {
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
                Series = new[]
                {
                        new
            {
                Name = "NG",
                Type = "bar",
                Data = data.Select(x => x.NG).ToArray(),
                Color = "#FF4560",
                Stack = "x",
                BarWidth = "15"
            },
                    new
            {
                Name = "OK",
                Type = "bar",
                Data = data.Select(x => x.OK).ToArray(),
                Color = "#05CD99",
                Stack = "x",
                BarWidth = "15"
            }

        },
                Grid = new
                {
                    y2 = 25
                }
            };
        }
    }
}
