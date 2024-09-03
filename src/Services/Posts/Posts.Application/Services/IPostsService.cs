using Bloggest.Common;
using Posts.Application.Dtos;

namespace Posts.Application.Services;

public interface IPostsService
{
    Task<PagedResult<PostDto>> GetPosts(PaginationOptions paginationOptions);
}