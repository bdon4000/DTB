using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DTB.Data.BatteryData.BaseModel
{
    [Index(nameof(ShellCode), IsUnique = true)]
    public class ShellBaseModel:BaseModel
    {
        [MaxLength(30)]
        public string ShellCode { get; set; }
    }
}
