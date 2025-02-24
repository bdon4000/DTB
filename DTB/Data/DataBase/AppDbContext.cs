using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DTB.Data.BatteryData;
using DTB.Data.Devices;

namespace DTB.Data.DataBase
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Shift> Shifts { get; set; }
    }
}
