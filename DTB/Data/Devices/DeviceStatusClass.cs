using DTB.Data.BatteryData.BaseModel;

namespace DTB.Data.Devices
{
    public class DeviceStatusClass
    {
        public DeviceModel? DeviceInfo { set; get; }
        public DateTime LastStateChangeTime { get; set; }

        public DateTime LastStatusTransitionTime { get; set; }
        public int LastState { get; set; }
        public DateTime UpdateTime { set; get; }
        public string? Shift { set; get; }
        public string? Operator { set; get; }
        public int Status { set; get; }
        public string? StatusMsg { set; get; }
        public string ErrorMsg { set; get; }
        public List<float> chartdatas { set; get; }
        public float LastElectricMeter { get; set; } = 0;
        public float PPM { set; get; }
        public Queue<float> PPMHistory { get; set; } = new Queue<float>();
        public float SmoothedPPM { get; set; }
        public List<FullBaseModel> BatteryDataBuff { set; get; }

        public List<DeviceChartData> deviceChartDatas { set; get; }

        public Dictionary<string, int> NgReasonMap { get; set; } = new Dictionary<string, int>();

        public DeviceStatusClass()
        {
            NgReasonMap = new Dictionary<string, int>
        {
            { "Unknown", 0 } // 初始化时添加"Unknown"作为第一个NG原因
        };
        }
    }
 
    public class DeviceChartData
    {
        public DateTime StartTime { set; get; }

        public int OkOutput { set; get; }
        public int NgOutput { set; get; }

        private List<int> _ngStatistics;
        public float[] StateStatistics { set; get; }
        public List<int> NgStatistics
        {
            get => _ngStatistics;
            set => _ngStatistics = value;
        }
        public float OEE { set; get; }
        public float CPK { set; get; }
        public float ElectricMeter { get; set; }
        public float PPM { set; get; }

        public DeviceChartData()
        {
            StateStatistics = new float[6];
            _ngStatistics = new List<int>() { 0 };
        }
        public void EnsureNgStatisticsCapacity(int count)
        {
            while (_ngStatistics.Count < count)
            {
                _ngStatistics.Add(0);
            }
        }
    }
}
