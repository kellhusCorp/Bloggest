using Bloggest.Common;

namespace Posts.Domain.Entities;

public class TagDbo : IBaseEntityDbo
{
    public Guid Id { get; init; }
    
    public string Name { get; set; } = null!;
}