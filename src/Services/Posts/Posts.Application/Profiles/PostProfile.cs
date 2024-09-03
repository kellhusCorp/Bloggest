using AutoMapper;
using Posts.Application.Dtos;
using Posts.Domain.Entities;

namespace Posts.Application.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostDbo, PostDto>();
    }
}