using DTB.Data.BatteryData;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class BatteryRelation
{
    [Key]
    public int Id { get; set; }

    [MaxLength(30)]
    public string? JellyCode { get; set; }

    [MaxLength(30)]
    public string? ShellCode { get; set; }

    [MaxLength(30)]
    public string? FilmCode { get; set; }
}

// 配置类用于设置索引和外键关系
public class BatteryRelationConfiguration : IEntityTypeConfiguration<BatteryRelation>
{
    public void Configure(EntityTypeBuilder<BatteryRelation> builder)
    {
        // 设置索引
        builder.HasIndex(x => x.JellyCode);
        builder.HasIndex(x => x.ShellCode);
        builder.HasIndex(x => x.FilmCode);

    }
}