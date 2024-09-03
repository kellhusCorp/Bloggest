using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bloggest.Common;
using Bloggest.Common.Extensions;
using Posts.Application.Dtos;
using Posts.Infrastructure.Contexts;

namespace Posts.Application.Services;

public class PostsService : IPostsService
{
    private readonly PostsContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PostsService(PostsContext dbContext, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<PagedResult<PostDto>> GetPosts(PaginationOptions paginationOptions)
    {
        var (posts, count) = await _dbContext.Posts
            .Where(x => !x.IsDraft && x.PublishedAt <= _dateTimeProvider.Now)
            .OrderByDescending(x => x.PublishedAt)
            .ProjectTo<PostDto>(_mapper.ConfigurationProvider)
            .GetPagedAsync(paginationOptions);
        
        return new PagedResult<PostDto>(posts, count);
    }
}