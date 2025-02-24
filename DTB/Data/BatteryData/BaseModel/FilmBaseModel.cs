using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace DTB.Data.BatteryData.BaseModel
{
    [Index(nameof(FilmCode), IsUnique = true,Name = "IX_FilmCode")]
    public class FilmBaseModel:BaseModel
    {
        [MaxLength(30)]
        public string FilmCode { get; set; }


    }
}