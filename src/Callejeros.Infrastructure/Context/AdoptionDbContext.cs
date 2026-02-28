using System.Data;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Domain.Events;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adoption.Infrastructure.Context;

public class AdoptionDbContextFactory : IDesignTimeDbContextFactory<AdoptionDbContext>
{

    public AdoptionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AdoptionDbContext>();
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Callejeros.API"));

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
    : IdentityDbContext<ApplicationUser>, IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;
    private IDomainEventDispatcher _domainEventDispatcher;
    public bool HasActiveTransaction => _currentTransaction != null;
    public IDbContextTransaction? GetCurrentTransaction => _currentTransaction;
    public AdoptionDbContext(DbContextOptions<AdoptionDbContext> options) : base(options) {}
    public AdoptionDbContext(
        DbContextOptions<AdoptionDbContext> options,
        IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }


    public DbSet<Animal> Animals { get; set; }
    public DbSet<AdoptionRequest> AdoptionRequests { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = GetDomainEvents();
        
        await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
        
        foreach (var entity in ChangeTracker.Entries<Entity>())
        {
            entity.Entity.ClearDomainEvents();
        }
        
        _ = await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        return ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("adoption");
        modelBuilder.ApplyConfiguration(new AdoptionRequestEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AnimalEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return this.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction?> StartTransactionAsync()
    {
        if (_currentTransaction != null) return null;
        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }
    }

}
