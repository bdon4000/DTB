using DTB.Data.BatteryData.BaseModel;

namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private int TotalOutput => deviceStatus?.deviceChartDatas?
        .Sum(x => x.OkOutput + x.NgOutput) ?? 0;
        private float TotalElectricMeter => deviceStatus?.deviceChartDatas?
        .Sum(x => x.ElectricMeter) ?? 0;
        private int TotalOkOutput => deviceStatus?.deviceChartDatas?
            .Sum(x => x.OkOutput) ?? 0;
        private int TotalNgOutput => deviceStatus?.deviceChartDatas?
            .Sum(x => x.NgOutput) ?? 0;
        private int TotalProduction => deviceStatus?.deviceChartDatas?
        .Sum(x => x.OkOutput + x.NgOutput) ?? 0;
        private string TotalNgRate => ((float)TotalNgOutput / (TotalProduction > 0 ? TotalProduction : 1) * 100).ToString("F1");
        // 优化 CPK 计算方法
        private double CalculateCpk(List<FullBaseModel> data, string fieldName, float LSL, float USL)
        {
            var values = data.Select(item =>
            {
                var value = GetParameterValue(item, fieldName);
                return value is float f ? f : (float?)null;
            })
            .Where(v => v.HasValue)
            .Select(v => v.Value)
            .ToList();

            if (!values.Any()) return 0;

            var mean = values.Average();
            var stdDev = Math.Sqrt(values.Sum(v => Math.Pow(v - mean, 2)) / values.Count);

            if (stdDev == 0) return 0;

            var cp = (USL - LSL) / (6 * stdDev);
            var ca = (mean - ((USL + LSL) / 2)) / ((USL - LSL) / 2);

            return cp * (1 - Math.Abs(ca));
        }
        private object GetParameterValue(FullBaseModel data, string fieldName)
        {
            return fieldName switch
            {
                nameof(FullBaseModel.Param1) => data.Param1,
                nameof(FullBaseModel.Param2) => data.Param2,
                nameof(FullBaseModel.Param3) => data.Param3,
                nameof(FullBaseModel.Param4) => data.Param4,
                nameof(FullBaseModel.Param5) => data.Param5,
                nameof(FullBaseModel.Param6) => data.Param6,
                nameof(FullBaseModel.Param7) => data.Param7,
                nameof(FullBaseModel.Param8) => data.Param8,
                nameof(FullBaseModel.Param9) => data.Param9,
                nameof(FullBaseModel.Param10) => data.Param10,
                nameof(FullBaseModel.Param11) => data.Param11,
                nameof(FullBaseModel.Param12) => data.Param12,
                nameof(FullBaseModel.Param13) => data.Param13,
                nameof(FullBaseModel.Param14) => data.Param14,
                nameof(FullBaseModel.Param15) => data.Param15,
                nameof(FullBaseModel.Param16) => data.Param16,
                nameof(FullBaseModel.Param17) => data.Param17,
                nameof(FullBaseModel.Param18) => data.Param18,
                nameof(FullBaseModel.Param19) => data.Param19,
                nameof(FullBaseModel.Param20) => data.Param20,
                nameof(FullBaseModel.FilmCode) => data.FilmCode,
                nameof(FullBaseModel.JellyCode) => data.JellyCode,
                nameof(FullBaseModel.ShellCode) => data.ShellCode,
                nameof(FullBaseModel.stringParam3) => data.stringParam3,
                nameof(FullBaseModel.stringParam4) => data.stringParam4,
                nameof(FullBaseModel.stringParam5) => data.stringParam5,
                nameof(FullBaseModel.stringParam6) => data.stringParam6,
                nameof(FullBaseModel.stringParam7) => data.stringParam7,
                nameof(FullBaseModel.stringParam8) => data.stringParam8,
                _ => null
            };
        }
    }
}
