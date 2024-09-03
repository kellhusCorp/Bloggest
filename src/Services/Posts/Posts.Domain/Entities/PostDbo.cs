using Bloggest.Common;

namespace Posts.Domain.Entities;

public sealed class PostDbo : IBaseEntityDbo
{
    public Guid Id { get; init; }
    
    public string Header { get; set; } = null!;

    public string Link { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsDraft { get; set; }
    
    public DateTimeOffset? PublishedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public string? AuthorId { get; set; } = null!;

    public ICollection<TagAssignmentDbo> TagAssignments {get; set;} = [];
}