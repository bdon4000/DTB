using DTB.Data.App.Status;

namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {

        private object _cpkChartOption;
        private string _selectedCpkField;
        private double _cp;
        private double _ca;
        private double _cpk;
 
        private void UpdateCpkChartOption()
        {
            if (deviceStatus?.DeviceInfo?.DeviceData == null || deviceStatus.BatteryDataBuff == null)
            {
                _cpkChartOption = null;
                return;
            }

            var cpkField = string.IsNullOrEmpty(_selectedCpkField)
                ? deviceStatus.DeviceInfo.DeviceData.FirstOrDefault(f => f.isCPK)
                : deviceStatus.DeviceInfo.DeviceData.FirstOrDefault(f => f.fieldName == _selectedCpkField);

            if (cpkField == null || !cpkField.LSL.HasValue || !cpkField.USL.HasValue)
            {
                _cpkChartOption = null;
                return;
            }

            var LSL = cpkField.LSL.Value;
            var USL = cpkField.USL.Value;

            // 提取并过滤数据
            var values = deviceStatus.BatteryDataBuff
                .Select(item =>
                {
                    var value = GetParameterValue(item, cpkField.fieldName);
                    return value is float f ? f : (float?)null;
                })
                .Where(v => v.HasValue && v.Value >= LSL && v.Value <= USL)
                .Select(v => v.Value)
                .ToList();

            if (!values.Any() || values.Count < 2) // 确保至少有两个数据点
            {
                _cpkChartOption = null;
                return;
            }

            var mean = values.Average();
            var stdDev = Math.Sqrt(values.Sum(v => Math.Pow(v - mean, 2)) / values.Count);

            // 检查标准差是否为0或接近0
            if (stdDev < 0.000001)
            {
                _cpkChartOption = null;
                return;
            }

            // 计算直方图数据
            var binCount = Math.Min(15, Math.Max(5, (int)Math.Ceiling(Math.Sqrt(values.Count))));
            var min = Math.Max(LSL, values.Min());
            var max = Math.Min(USL, values.Max());

            // 检查min和max是否相等
            if (Math.Abs(max - min) < 0.000001)
            {
                _cpkChartOption = null;
                return;
            }

            var binWidth = (max - min) / binCount;

            var histogram = new float[binCount];
            foreach (var value in values)
            {
                var binIndex = Math.Min((int)((value - min) / binWidth), binCount - 1);
                histogram[binIndex]++;
            }

            // 生成直方图数据
            var histogramData = new object[binCount];
            for (int i = 0; i < binCount; i++)
            {
                var x = min + (i + 0.5) * binWidth;
                // 避免除以0
                var density = values.Count > 0 ? histogram[i] / (values.Count * binWidth) : 0;
                // 检查density是否为有限数
                if (!double.IsFinite(density))
                {
                    density = 0;
                }
                histogramData[i] = new object[] { x, density };
            }

            // 生成正态分布曲线
            var normalPoints = 30;
            var normalData = new object[normalPoints];
            var normalScale = stdDev > 0 ? 1.0 / (stdDev * Math.Sqrt(2 * Math.PI)) : 0;

            for (int i = 0; i < normalPoints; i++)
            {
                var x = LSL + (USL - LSL) * i / (normalPoints - 1);
                var y = normalScale * Math.Exp(-Math.Pow(x - mean, 2) / (2 * stdDev * stdDev));
                // 检查y是否为有限数
                if (!double.IsFinite(y))
                {
                    y = 0;
                }
                normalData[i] = new object[] { x, y };
            }

            // 计算 CPK 相关指标，添加安全检查
            var cp = stdDev > 0 ? (USL - LSL) / (6 * stdDev) : 0;
            var ca = (USL - LSL) != 0 ? (mean - ((USL + LSL) / 2)) / ((USL - LSL) / 2) : 0;
            var cpk = double.IsFinite(cp) && double.IsFinite(ca) ? cp * (1 - Math.Abs(ca)) : 0;
            _cp = cp;
            _ca = ca;
            _cpk = cpk;

            _cpkChartOption = new
            {
                tooltip = new { trigger = "axis" },
                grid = new { left = "10%", right = "10%", top = "20", bottom = "10%" },
                xAxis = new
                {
                    type = "value",
                    //name = cpkField.name,
                    //axisLabel = new { formatter = "function(value) { return value.toFixed(2); }" },
                    splitLine = new { show = false },
                    min = LSL,
                    max = USL,
                    markLine = new
                    {
                        silent = true,
                        symbol = "none",
                        data = new[]
            {
                new { xAxis = LSL, name = "LSL", lineStyle = new { color = "#FF4560" } },
                new { xAxis = USL, name = "USL", lineStyle = new { color = "#FF4560" } }
            }
                    }
                },
                yAxis = new { type = "value", splitLine = new { show = false } },
                series = new object[]
    {
        new
        {
            type = "bar",
            name = "Distribution",
            barWidth = "90%",
            data = histogramData,
            itemStyle = new { color = "#4CAF50", opacity = 0.7 }
        },
        new
        {
            type = "line",
            name = "Normal",
            smooth = true,
            symbol = "none",
            data = normalData,
            itemStyle = new { color = "#2196F3" },
            areaStyle = new { opacity = 0.2 }
        }
    },
                legend = new { show = false }
            };
        }

        private object GetPPMGaugeChart(float currentPPM, float standardPPM)
        {
            return new
            {
                Series = new[]
                {
            new
            {
                Type = "gauge",
                Min = 0,
                Max = standardPPM * 1.2,
                //Center = new[] { "50%", "55%" },  // 调整中心位置
                Radius = "100%",  // 调整大小
                StartAngle = 225,
                EndAngle = -45,
                AxisLine = new
                {
                    LineStyle = new
                    {
                        Width = 15,  // 减小轴线宽度
                        Color = new object[]
                        {
                            new object[] { 0.5d, "#FF4560" },
                            new object[] { 0.8d, "#FEB019" },
                            new object[] { 1.0d, "#05CD99" }

                        }
                    }
                },
                Pointer = new
                {
                    Width = 5,  // 减小指针宽度
                    Length = "100%",  // 调整指针长度
                    ItemStyle = new
                    {
                        Color = "auto"
                    }
                },
                AxisTick = new
                {
                    Show = false,
                    Distance = -30,  // 调整刻度位置
                    Length = 2,      // 减小刻度长度
                    LineStyle = new
                    {
                        Color = "#fff",
                        Width = 0
                    }
                },
                AxisLabel = new
                {
                    Show = false,
                    Color = "inherit",
                    Distance = 25,  // 调整标签距离
                    FontSize = 12   // 减小字体大小
                },
                Detail = new
                {
                    ValueAnimation = true,
                    Formatter = "{value}",
                    Color = "inherit",
                    FontSize = 20,
                    OffsetCenter = new object[] { 0, "70%" }  // 调整数值位置
                },
                Data = new[]
                {
                    new
                    {
                        Value = Math.Round(currentPPM,1)
                    }
                }
            }
        }
            };
        }

    }
}
