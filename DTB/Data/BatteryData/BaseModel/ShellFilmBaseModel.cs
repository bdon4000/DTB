using Microsoft.EntityFrameworkCore;

namespace DTB.Data.BatteryData.BaseModel
{
    [Index(nameof(FilmCode), IsUnique = true)]
    public class ShellFilmBaseModel:BaseModel

    {
        [MaxLength(30)]
        public string ShellCode { get; set; }

        [MaxLength(30)]
        public string FilmCode { get; set; }

    }
}
