using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Adoption.Infrastructure.Context;

public class AdoptionDbContextFactory : IDesignTimeDbContextFactory<AdoptionDbContext>
{

    public AdoptionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AdoptionDbContext>();
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Api"));

        Console.WriteLine($"Base Path: {basePath}");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        return new AdoptionDbContext(optionsBuilder.Options);
    }
}

public class AdoptionDbContext
    : DbContext, IUnitOfWork
{
    private readonly IMediator? _mediator;
    public AdoptionDbContext(DbContextOptions<AdoptionDbContext> options) : base(options)
    { }
    public AdoptionDbContext(DbContextOptions<AdoptionDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public DbSet<Animal> Animals { get; set; }
    public DbSet<AdoptionRequest> AdoptionRequests { get; set; }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator!.DispatchDomainEventsAsync(this);
        _ = await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("adoption");
        modelBuilder.ApplyConfiguration(new AdoptionRequestEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AnimalEntityTypeConfiguration());

    }

    Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return this.SaveChangesAsync(cancellationToken);
    }
}
