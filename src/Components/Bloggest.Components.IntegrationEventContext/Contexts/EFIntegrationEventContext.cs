using Bloggest.Components.Bus.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bloggest.Components.IntegrationEventContext.Contexts;

public class EFIntegrationEventContext : DbContext
{
    public EFIntegrationEventContext(DbContextOptions<EFIntegrationEventContext> options) : base(options)
    {
        
    }
    
    public DbSet<IntegrationEventEntry> IntegrationEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IntegrationEventEntry>(ConfigureIntegrationEventLogEntry);
    }
    
    void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventEntry> builder)
    {
        builder.ToTable("IntegrationEvents");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.State)
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .IsRequired();

        builder.Property(e => e.EventTypeName)
            .IsRequired();
    }
}