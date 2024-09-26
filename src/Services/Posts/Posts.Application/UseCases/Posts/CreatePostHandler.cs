﻿using Bloggest.Components.Bus.Contracts.Interfaces;
using Ganss.Xss;
using MediatR;
using Posts.Application.IntegrationEvents;
using Posts.Domain.Entities;
using Posts.Infrastructure.Contexts;

namespace Posts.Application.UseCases.Posts;

public class CreatePostHandler : IRequestHandler<CreatePostCommand, bool>
{
    private readonly PostsContext _postsContext;
    private readonly IIntegrationEventService _integrationEventService;

    public CreatePostHandler(PostsContext postsContext, IIntegrationEventService integrationEventService)
    {
        _postsContext = postsContext;
        _integrationEventService = integrationEventService;
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
            Link = "how-to-generate-permalink",
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
        
        await _integrationEventService.SaveEventAsync(postCreatedEvent, transaction);
        
        await _postsContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}