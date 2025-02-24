namespace DTB.Data.Devices
{
    public class Shift
    {
        public int Id { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int PlanOutput { get; set; } = 0;
    }
}
