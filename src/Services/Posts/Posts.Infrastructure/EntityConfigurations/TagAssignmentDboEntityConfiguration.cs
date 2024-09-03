using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Posts.Domain.Entities;

namespace Posts.Infrastructure.EntityConfigurations;

public class TagAssignmentDboEntityConfiguration : IEntityTypeConfiguration<TagAssignmentDbo>
{
    public void Configure(EntityTypeBuilder<TagAssignmentDbo> builder)
    {
        builder.HasKey(e => new { e.PostId, e.TagId });
    }
}