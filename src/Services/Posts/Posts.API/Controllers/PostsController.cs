using Bloggest.Common;
using Microsoft.AspNetCore.Mvc;
using Posts.Application.Services;

namespace Posts.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostsService _postsService;

    public PostsController(IPostsService postsService)
    {
        _postsService = postsService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] PaginationOptions paginationOptions)
    {
        var posts = await _postsService.GetPosts(paginationOptions);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        throw new NotImplementedException();
    }
}