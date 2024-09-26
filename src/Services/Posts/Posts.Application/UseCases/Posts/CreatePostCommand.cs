using MediatR;

namespace Posts.Application.UseCases.Posts;

public class CreatePostCommand : IRequest<bool>
{
    public string Header { get; init; }

    public Guid AuthorId { get; init; }

    public IEnumerable<Guid> TagIds { get; init; }
}