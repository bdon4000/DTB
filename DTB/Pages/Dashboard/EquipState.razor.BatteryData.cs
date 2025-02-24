using DTB.Data.BatteryData.BaseModel;

namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private void InitializeBatteryDataHeaders()
        {
            _batteryDataHeaders.Clear();
            _batteryDataHeaders.Add(new() { Text = "Time", Value = nameof(FullBaseModel.updateTime), Width = "0px" });
            _batteryDataHeaders.Add(new() { Text = "Result", Value = nameof(FullBaseModel.result), Width = "0px" });

            // 根据设备类型添加相应的代码列
            var deviceType = deviceStatus?.DeviceInfo?.DeviceCode?.ToLower();
            switch (deviceType)
            {
                // Jelly相关设备
                case "jellyfeeding":
                case "biinserting":
                    _batteryDataHeaders.Add(new() { Text = "Jelly Code", Value = nameof(FullBaseModel.JellyCode), Width = "0px" });
                    break;

                // Shell和Jelly关系设备
                case "shellinserting":
                    _batteryDataHeaders.Add(new() { Text = "Jelly Code", Value = nameof(FullBaseModel.JellyCode), Width = "0px" });
                    _batteryDataHeaders.Add(new() { Text = "Shell Code", Value = nameof(FullBaseModel.ShellCode), Width = "0px" });
                    break;

                // 只有Shell的设备
                case "bottomwelding1":
                case "bottomwelding2":
                case "necking":
                case "tiinserting":
                case "beading":
                case "shortcircuittest":
                case "xray":
                case "injecting":
                case "capwelding":
                case "sealing":
                case "plasticfilming":
                case "filmshrinking":
                    _batteryDataHeaders.Add(new() { Text = "Shell Code", Value = nameof(FullBaseModel.ShellCode), Width = "0px" });
                    break;

                // Shell和Film关系设备
                case "inkjetprinting":
                    _batteryDataHeaders.Add(new() { Text = "Shell Code", Value = nameof(FullBaseModel.ShellCode), Width = "0px" });
                    _batteryDataHeaders.Add(new() { Text = "Film Code", Value = nameof(FullBaseModel.FilmCode), Width = "0px" });
                    break;

                // 只有Film的设备
                case "appearanceinspection":
                case "precharge":
                    _batteryDataHeaders.Add(new() { Text = "Film Code", Value = nameof(FullBaseModel.FilmCode), Width = "0px" });
                    break;
            }

            // 添加设备特定的数据字段
            if (deviceStatus?.DeviceInfo?.DeviceData != null)
            {
                foreach (var field in deviceStatus.DeviceInfo.DeviceData.OrderBy(f => f.fieldName))
                {
                    var headerText = field.name;
                    if (field.isCPK && field.LSL.HasValue && field.USL.HasValue)
                    {
                        headerText += $"\n({field.LSL.Value:F1}-{field.USL.Value:F1})";
                    }

                    _batteryDataHeaders.Add(new()
                    {
                        Text = headerText,
                        Value = field.fieldName,
                        Width = "0px"
                    });
                }
            }
        }
    }
}
