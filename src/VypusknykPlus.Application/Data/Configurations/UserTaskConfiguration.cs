using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class UserTaskConfiguration : IEntityTypeConfiguration<UserTask>
{
    public void Configure(EntityTypeBuilder<UserTask> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.TargetValue).HasPrecision(10, 2);

        builder.HasOne(t => t.TargetCategory)
            .WithMany()
            .HasForeignKey(t => t.TargetCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.RewardPromoCode)
            .WithMany()
            .HasForeignKey(t => t.RewardPromoCodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class UserTaskProgressConfiguration : IEntityTypeConfiguration<UserTaskProgress>
{
    public void Configure(EntityTypeBuilder<UserTaskProgress> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Progress).HasPrecision(10, 2);

        builder.HasOne(p => p.Task)
            .WithMany(t => t.Progresses)
            .HasForeignKey(p => p.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.AwardedCard)
            .WithMany()
            .HasForeignKey(p => p.AwardedCardId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => new { p.TaskId, p.UserId }).IsUnique();
    }
}
