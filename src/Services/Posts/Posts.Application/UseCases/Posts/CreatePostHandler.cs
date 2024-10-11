using Ganss.Xss;
using MediatR;
using Posts.Application.IntegrationEvents;
using Posts.Application.Services;
using Posts.Domain.Entities;
using Posts.Infrastructure.Contexts;

namespace Posts.Application.UseCases.Posts;

public class CreatePostHandler : IRequestHandler<CreatePostCommand, bool>
{
    private readonly PostsContext _postsContext;
    private readonly PostsIntegrationEventService _postsIntegrationEventService;
    private readonly LinkGenerator _linkGenerator;

    public CreatePostHandler(PostsContext postsContext, PostsIntegrationEventService postsIntegrationEventService, LinkGenerator linkGenerator)
    {
        _postsContext = postsContext;
        _postsIntegrationEventService = postsIntegrationEventService;
        _linkGenerator = linkGenerator;
    }
    
    public async Task<bool> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var sanitizer = new HtmlSanitizer();

        var postDbo = new PostDbo
        {
            Content = sanitizer.Sanitize(command.Body),
            Header = command.Header,
            AuthorId = command.AuthorId,
            IsDraft = command.IsDraft,
            Link = _linkGenerator.Generate(command.Header),
            TagAssignments = command.TagIds.Select(x => new TagAssignmentDbo { TagId = x }).ToList()
        };

        await using var transaction = await _postsContext.Database.BeginTransactionAsync(cancellationToken);
        
        await _postsContext.Posts.AddAsync(postDbo, cancellationToken);
        
        var postCreatedEvent = new PostCreatedIntegrationEvent
        {
            PostId = postDbo.Id,
            Header = postDbo.Header,
            AuthorId = postDbo.AuthorId,
            TagIds = postDbo.TagAssignments.Select(x => x.TagId)
        };
        
        await _postsIntegrationEventService.AddAndSaveEventAsync(postCreatedEvent);
        
        await _postsContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        await _postsIntegrationEventService.PublishEventsThroughEventBusAsync(transaction.TransactionId);
        
        return true;
    }
}