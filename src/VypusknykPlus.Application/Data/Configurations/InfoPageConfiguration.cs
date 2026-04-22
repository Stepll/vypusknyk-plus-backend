using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class InfoPageConfiguration : IEntityTypeConfiguration<InfoPage>
{
    public void Configure(EntityTypeBuilder<InfoPage> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Slug).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Content).IsRequired();
        builder.HasIndex(p => p.Slug).IsUnique();
    }
}
