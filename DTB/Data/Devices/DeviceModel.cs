 namespace DTB.Data.Devices
{
    public class DeviceModel
    {
        public int Id { set; get; }
        public string? DeviceName { set; get; }
        public string? DeviceCode { set; get; }
        public string? DeviceAvatar { set; get; }
        public dataField[]? DeviceData { set; get; }
        public float? StandardPPM { set; get; }
    }
    public class dataField
    {
        public string? name { set; get; }
        public string? fieldName { set; get; }

        public bool isCPK { set; get; }

        public float? USL { set; get; }

        public float? LSL { set; get; }

    }
}