using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using DTB.Data.BatteryData;
using DTB.Data.Devices;
using System.Text.Json;
using System.Reflection.PortableExecutable;

namespace DTB.Data
{
    public class BatteryDbContext : DbContext
    {
        public BatteryDbContext(DbContextOptions<BatteryDbContext> options) : base(options)
        {

        }

        public DbSet<BatteryRelation> BatteryRelations { get; set; }
        public DbSet<JellyFeedingData> JellyFeedingDatas { get; set; }
        public DbSet<BiInsertingData> BiInsertingDatas { get; set; }
        public DbSet<ShellInsertingData> ShellInsertingDatas { get; set; }
        public DbSet<BottomWelding1Data> BottomWelding1Datas { get; set; }
        public DbSet<BottomWelding2Data> BottomWelding2Datas { get; set; }
        public DbSet<NeckingData> NeckingDatas { get; set; }
        public DbSet<TiInsertingData> TiInsertingDatas { get; set; }
        public DbSet<BeadingData> BeadingDatas { get; set; }
        public DbSet<ShortCircuitTestData> ShortCircuitTestDatas { get; set; }
        public DbSet<XRAYData> XRAYDatas { get; set; }
        public DbSet<InjectingData> InjectingDatas { get; set; }
        public DbSet<CapWeldingData> CapWeldingDatas { get; set; }
        public DbSet<SealingData> SealingDatas { get; set; }
        public DbSet<PlasticFilmingData> PlasticFilmingDatas { get; set; }
        public DbSet<FilmShrinkingData> FilmShrinkingDatas { get; set; }
        public DbSet<InkjetPrintingData> InkjetPrintingDatas { get; set; }
        public DbSet<AppearanceInspectionData> AppearanceInspectionDatas { get; set; }
        public DbSet<PreChargeData> PreChargeDatas { get; set; }


        public DbSet<DeviceModel> Devices { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<DeviceModel>().ToTable("Devices", t => t.ExcludeFromMigrations());

            _ = modelBuilder.Entity<DeviceModel>(static entity =>
            {
                entity.ToTable("Devices"); // 指定表名
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.DeviceCode).IsUnique();
                _ = entity.Property(e => e.DeviceData)
          .HasConversion(
              v => v == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(v),
              v => v == null ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<dataField[]>(v)
          )
           .HasColumnType("NVARCHAR(MAX)");
            });

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BatteryRelationConfiguration());

        }
    }

}