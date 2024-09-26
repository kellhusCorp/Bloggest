using System.Reflection;
using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Extensions.DependencyInjection;
using Posts.Application.Services;

namespace Posts.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeProvider, DefaultDateTimeProvider>();
        services.AddScoped<IPostsService, PostsService>();
        
        #region AutoMapper

        var mapperConfig = new MapperConfiguration(options =>
        {
            options.AddMaps(Assembly.GetExecutingAssembly());
            options.Internal().RecursiveQueriesMaxDepth = 10;
        });
        
        IMapper mapper = mapperConfig.CreateMapper();

        services.AddSingleton(mapper);

        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        #endregion
    }
}