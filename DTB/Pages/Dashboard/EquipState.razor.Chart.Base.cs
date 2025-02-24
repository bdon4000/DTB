namespace DTB.Pages.Dashboard
{
    public partial class EquipState
    {
        private Dictionary<string, object> _chartEvents = new();
        public class ChartClickParams
        {
            public string ComponentType { get; set; }
            public string Name { get; set; }
        }
    }
}
