using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DTB.Data.BatteryData.BaseModel
{
    [Index(nameof(JellyCode), IsUnique = true)]
    public class JellyBaseModel: BaseModel
    {

        [MaxLength(30)]
        public string JellyCode { get; set; }

    }
}
