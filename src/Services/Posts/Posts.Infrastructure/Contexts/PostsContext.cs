﻿using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;

namespace Posts.Infrastructure.Contexts;

public class PostsContext : DbContext
{
    public PostsContext(DbContextOptions<PostsContext> options) : base(options)
    {
        
    }
    
    public DbSet<PostDbo> Posts { get; set; }
    
    public DbSet<TagDbo> Tags { get; set; }
    
    public DbSet<TagAssignmentDbo> TagAssignments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostsContext).Assembly);
    }
}