using MediatR;

namespace Posts.Application.UseCases.Posts;

public class CreatePostHandler : IRequestHandler<CreatePostCommand, bool>
{
    public CreatePostHandler()
    {
        
    }
    
    public Task<bool> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}