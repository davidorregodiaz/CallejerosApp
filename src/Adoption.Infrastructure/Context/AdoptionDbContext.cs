using System.Data;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.SeedWork;
using Adoption.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace Adoption.Infrastructure.Context;

public class AdoptionDbContextFactory : IDesignTimeDbContextFactory<AdoptionDbContext>
{

    public AdoptionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AdoptionDbContext>();
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Adoption.API"));

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
    private IDbContextTransaction? _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction != null;
    public IDbContextTransaction? GetCurrentTransaction => _currentTransaction;
    public AdoptionDbContext(DbContextOptions<AdoptionDbContext> options) : base(options)
    { }

    public DbSet<Animal> Animals { get; set; }
    public DbSet<AdoptionRequest> AdoptionRequests { get; set; }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // TODO event dispatcher personalizado
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
