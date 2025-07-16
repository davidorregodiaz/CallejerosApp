using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Infraestructure.Context;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{

    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Api"));

        Console.WriteLine($"Base Path: {basePath}");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection")); 

        return new AppDbContext(optionsBuilder.Options);
    }
}

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext() { }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Animal> Animals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        SetupDatabaseModels(builder);
        base.OnModelCreating(builder);
    }


    private void SetupDatabaseModels(ModelBuilder builder)
    {
        builder.Entity<Animal>(e =>
        {
            e.ToTable("animals");

            e.Property(a => a.Id)
                .HasColumnName("animal_id");

            e.Property(a => a.Name)
                .HasColumnName("name");

            e.Property(a => a.Age)
                .HasColumnName("age");

            e.OwnsMany(a => a.Requirements, r =>
            {
                r.ToTable("animal_requierements");
                r.WithOwner().HasForeignKey("animal_id");
                r.Property(r => r.Value)
                    .HasColumnName("requierement")
                    .IsRequired()
                    .HasMaxLength(50);
            });

            e.OwnsOne(a => a.Breed, b =>
            {
                b.WithOwner().HasForeignKey("animal_id");
                b.Property(b => b.Value)
                    .HasColumnName("breed")
                    .IsRequired()
                    .HasMaxLength(30);
                b.HasIndex(b => b.Value);
            });

            e.OwnsOne(a => a.AnimalType, t =>
            {
                t.Property(t => t.Value)
                    .HasColumnName("type")
                    .IsRequired()
                    .HasMaxLength(20);
                t.WithOwner()
                    .HasForeignKey("animal_id");
                t.HasIndex(t => t.Value);
            });

            e.HasOne(a => a.Owner)
                .WithMany(a => a.Animals)
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            //Propiedades de navegacion
            e.Navigation(e => e.AnimalType).Metadata.SetField("_type");
            e.Navigation(e => e.Breed).Metadata.SetField("_breed");
            e.Navigation(e => e.Requirements).Metadata.SetField("_requierements");

            e.HasIndex(a => a.Name);
            e.HasIndex(a => a.Age);
            e.HasIndex(a => a.OwnerId);

        });
    }
}