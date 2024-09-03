using Microsoft.EntityFrameworkCore;
using Posts.Infrastructure.Contexts;

namespace Posts.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<PostsContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.Run();
    }
}