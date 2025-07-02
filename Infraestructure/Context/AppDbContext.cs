using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Context;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        SetupDatabaseModels(builder);
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
            e.Navigation(e => e.Requirements).Metadata.SetField("_requierements");

            e.OwnsOne(a => a.Breed, b =>
            {
                b.ToTable("breeds");
                b.WithOwner().HasForeignKey("animal_id");
                b.Property(b => b.Value)
                    .HasColumnName("breed")
                    .IsRequired()
                    .HasMaxLength(30);
            });
            e.Navigation(e => e.Breed).Metadata.SetField("_breed");
        });
    }
}