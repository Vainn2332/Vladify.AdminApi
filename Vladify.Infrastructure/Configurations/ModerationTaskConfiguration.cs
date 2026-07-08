using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vladify.Domain.Constants;
using Vladify.Domain.Entities;

namespace Vladify.Infrastructure.Configurations;

public class ModerationTaskConfiguration : IEntityTypeConfiguration<ModerationTask>
{
    public void Configure(EntityTypeBuilder<ModerationTask> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Status).HasConversion<string>();
        builder.HasIndex(p => p.SongId).IsUnique();
        builder.Property(p => p.Message).HasMaxLength(DomainConstants.ModerationTask.MaxMessageLength);
    }
}
