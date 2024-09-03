using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Domain.Entities;

namespace Posts.Infrastructure.EntityConfigurations;

public class TagDboEntityConfiguration : IEntityTypeConfiguration<TagDbo>
{
    public void Configure(EntityTypeBuilder<TagDbo> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasMany(e => e.TagAssignments)
            .WithOne()
            .HasForeignKey(e => e.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}