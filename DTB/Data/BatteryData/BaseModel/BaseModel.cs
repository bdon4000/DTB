namespace DTB.Data.BatteryData.BaseModel
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }

        public bool result { get; set; }

        [MaxLength(20)] 
        public String? ngReason { get; set; }
        public DateTime uploadTime { get; set; }
        public DateTime updateTime { get; set; } = DateTime.Now;

        public float? Param1 { get; set; }
        public float? Param2 { get; set; }
        public float? Param3 { get; set; }
        public float? Param4 { get; set; }
        public float? Param5 { get; set; }
        public float? Param6 { get; set; }
        public float? Param7 { get; set; }
        public float? Param8 { get; set; }
        public float? Param9 { get; set; }
        public string? StringParam1 { get; set; }
        public string? StringParam2 { get; set; }

    }
}
