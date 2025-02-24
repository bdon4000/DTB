using DTB.Data.BatteryData.BaseModel;
using Microsoft.EntityFrameworkCore;

namespace DTB.Data.BatteryData
{
    public class JellyFeedingData : JellyBaseModel {
    public string? stringParam3 { set; get; }
    }
    public class BiInsertingData : JellyBaseModel { }
    public class ShellInsertingData : JellyShellBaseModel { }
    public class BottomWelding1Data : ShellBaseModel {
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
        public float? Param21 { get; set; }
        public float? Param22 { get; set; }
        public float? Param23 { get; set; }
        public float? Param24 { get; set; }
    }
    public class BottomWelding2Data : ShellBaseModel {
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
        public float? Param21 { get; set; }
        public float? Param22 { get; set; }
        public float? Param23 { get; set; }
        public float? Param24 { get; set; }
    }
    public class NeckingData : ShellBaseModel { }
    public class TiInsertingData : ShellBaseModel { }
    public class BeadingData : ShellBaseModel { }
    public class ShortCircuitTestData : ShellBaseModel { }
    public class XRAYData : ShellBaseModel { }
    public class InjectingData : ShellBaseModel { }
    public class CapWeldingData : ShellBaseModel { }
    public class SealingData : ShellBaseModel { }
    public class PlasticFilmingData : ShellBaseModel {
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
        public float? Param21 { get; set; }
        public float? Param22 { get; set; }
        public float? Param23 { get; set; }
        public float? Param24 { get; set; }
    }
    public class FilmShrinkingData : ShellBaseModel { }
    public class InkjetPrintingData : ShellFilmBaseModel { }
    public class AppearanceInspectionData : FilmBaseModel 
    {
        public string? StringParam3 { get; set; }
        public string? StringParam4 { get; set; }
        public string? StringParam5 { get; set; }
    }
    public class PreChargeData : FilmBaseModel { }

}