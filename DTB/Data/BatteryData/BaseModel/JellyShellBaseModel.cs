using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DTB.Data.BatteryData.BaseModel
{
    [Index(nameof(ShellCode), IsUnique = true)]
    public class JellyShellBaseModel:BaseModel
    {
        [MaxLength(30)]
        public string JellyCode { get; set; }

        [MaxLength(30)]
        public string ShellCode { get; set; }
    }
}
