using Bloggest.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Posts.Application.Services;
using Posts.Application.UseCases.Posts;

namespace Posts.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostsService _postsService;
    private readonly IMediator _mediator;

    public PostsController(IPostsService postsService, IMediator mediator)
    {
        _postsService = postsService;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] PaginationOptions paginationOptions)
    {
        var posts = await _postsService.GetPosts(paginationOptions);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePostCommand command)
    {
        var result = await _mediator.Send(command);
        return result ? NoContent() : BadRequest();
    }
}