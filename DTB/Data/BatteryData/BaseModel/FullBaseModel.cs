using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace DTB.Data.BatteryData.BaseModel
{
    public class FullBaseModel : BaseModel
    {
        public string FilmCode { get; set; }

        public string JellyCode { get; set; }

        public string ShellCode { get; set; }
        public float? Param10 { get; set; }
        public float? Param11 { get; set; }
        public float? Param12 { get; set; }
        public float? Param13 { get; set; }
        public float? Param14 { get; set; }
        public float? Param15 { get; set; }
        public float? Param16 { get; set; }
        public float? Param17 { get; set; }
        public float? Param18 { get; set; }
        public float? Param19 { get; set; }
        public float? Param20 { get; set; }

        public string? stringParam3 { set; get; }
        public string? stringParam4 { set; get; }
        public string? stringParam5 { set; get; }
        public string? stringParam6 { set; get; }
        public string? stringParam7 { set; get; }
        public string? stringParam8 { set; get; }
    }
}