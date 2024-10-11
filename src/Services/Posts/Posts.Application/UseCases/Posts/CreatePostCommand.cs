using MediatR;

namespace Posts.Application.UseCases.Posts;

public class CreatePostCommand : IRequest<bool>
{
    public string Header { get; init; }

    public string Body { get; init; }
    
    public string AuthorId { get; init; }
    
    public bool IsDraft { get; init; }

    public IEnumerable<Guid> TagIds { get; init; }
}