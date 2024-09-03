using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Domain.Entities;

namespace Posts.Infrastructure.EntityConfigurations;

public class PostDboEntityConfiguration : IEntityTypeConfiguration<PostDbo>
{
    public void Configure(EntityTypeBuilder<PostDbo> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasMany(e => e.TagAssignments)
            .WithOne()
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}