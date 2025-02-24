namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private object _ngChartOption = null;
        private Dictionary<string, bool> ngChartSelectedStates = new()
        {
            { "OK", false }// 其他 NG 原因的状态也会在这里
        };
        private void UpdateNgChartOption()
        {
            var ngData = new List<object>();

            // Add OK data with selection state
            ngData.Add(new
            {
                Value = TotalOkOutput,
                Name = "OK",
                ItemStyle = new { Color = "#05CD99" },
                Selected = selectedStates["OK"]
            });

            // Add NG data
            if (deviceStatus?.NgReasonMap != null)
            {
                foreach (var reason in deviceStatus.NgReasonMap)
                {
                    var ngCount = deviceStatus.deviceChartDatas?
                        .Sum(x => x.NgStatistics[reason.Value]) ?? 0;

                    if (ngCount > 0)
                    {
                        var name = string.IsNullOrEmpty(reason.Key) ? "Unknown" : reason.Key;

                        // Always show NG data
                        ngData.Add(new
                        {
                            Value = ngCount,
                            Name = name,
                            ItemStyle = new
                            {
                                Color = GetNgReasonColor(reason.Value),
                                BorderWidth = 2,
                                BorderColor = "#ffffff"
                            },
                            Selected = true  // NG data is always selected
                        });
                    }
                }
            }
            _ngChartOption = new
            {
                tooltip = new
                {
                    trigger = "item",
                    formatter = "{b}: {c} ({d}%)"
                },
                legend = new
                {
                    type = "scroll",
                    orient = "vertical",
                    right = "0",
                    top = "middle",
                    selectedMode = "multiple",
                    selected = selectedStates,
                    textStyle = new
                    {
                        fontSize = 10,
                        width = 50,
                        overflow = "truncate"
                    }
                },
                series = new[]
                {
                    new
                    {
                        name = "Production Data",
                        type = "pie",
                        radius = "70%",
                        center = new[] { "40%", "50%" },
                        data = ngData,
                        label = new { show = false },
                        selectedMode = false,  // Disable click selection
                        emphasis = new
                        {
                            itemStyle = new
                            {
                                shadowBlur = 10,
                                shadowOffsetX = 0,
                                shadowColor = "rgba(0, 0, 0, 0.5)"
                            }
                        }
                    }
                }
            };
        }
        private string GetNgReasonColor(int index)
        {
            // 使用更多样的颜色，完全避免绿色调
            string[] colors = new[]
            {
                "#FF4560",  // 红色
                "#FEB019",  // 橙色
                "#775DD0",  // 紫色
                "#FF6B6B",  // 珊瑚红
                "#008FFB",  // 蓝色
                "#F86624",  // 橘红
                "#4B4CFA",  // 靛蓝
                "#D10CE8",  // 紫红
                "#6C757D",  // 灰色
                "#1A73E8",  // 深蓝
                "#B15DFF",  // 淡紫
                "#FF9800",  // 深橙
            };
            return colors[index % colors.Length];
        }
    }
}
