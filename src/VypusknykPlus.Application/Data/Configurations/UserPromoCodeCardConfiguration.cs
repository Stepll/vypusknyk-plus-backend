using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class UserPromoCodeCardConfiguration : IEntityTypeConfiguration<UserPromoCodeCard>
{
    public void Configure(EntityTypeBuilder<UserPromoCodeCard> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.PromoCode)
            .WithMany(p => p.Cards)
            .HasForeignKey(c => c.PromoCodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.UserId, c.PromoCodeId }).IsUnique();
    }
}
