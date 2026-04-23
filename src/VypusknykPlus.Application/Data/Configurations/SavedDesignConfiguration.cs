using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class SavedDesignConfiguration : IEntityTypeConfiguration<SavedDesign>
{
    public void Configure(EntityTypeBuilder<SavedDesign> builder)
    {
        builder.HasKey(sd => sd.Id);

        builder.Property(sd => sd.DesignName).IsRequired().HasMaxLength(200);

        builder.OwnsOne(sd => sd.State, s =>
        {
            s.ToJson();
            s.Property(x => x.MainText).HasMaxLength(500);
            s.Property(x => x.School).HasMaxLength(500);
            s.Property(x => x.Comment).HasMaxLength(1000);
            s.Property(x => x.PrintType).HasMaxLength(50);
            s.Property(x => x.Color).HasMaxLength(50);
            s.Property(x => x.Material).HasMaxLength(50);
            s.Property(x => x.TextColor).HasMaxLength(50);
            s.Property(x => x.ExtraTextColor).HasMaxLength(50);
            s.Property(x => x.Font).HasMaxLength(50);
            s.OwnsMany(x => x.Classes, c =>
            {
                c.Property(x => x.ClassName).HasMaxLength(200);
                c.Property(x => x.Names).HasMaxLength(10000);
            });
        });

        builder.HasOne(sd => sd.User)
            .WithMany(u => u.SavedDesigns)
            .HasForeignKey(sd => sd.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sd => sd.UserId);
    }
}
