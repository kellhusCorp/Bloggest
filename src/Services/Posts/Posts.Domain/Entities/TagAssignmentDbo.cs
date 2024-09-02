namespace Posts.Domain.Entities;

public sealed class TagAssignmentDbo
{
    public Guid PostId { get; set; }
    
    public Guid TagId { get; set; }
}