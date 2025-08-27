using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adoption.Infrastructure.EntityConfigurations;

public class AnimalEntityTypeConfiguration 
    : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> animalConfiguration)
    {
        animalConfiguration.ToTable("Animals");

        animalConfiguration.Ignore(a => a.DomainEvents);

        animalConfiguration.HasKey(a => a.Id);

        animalConfiguration.Property(a => a.Id)
            .HasConversion(
                animalId => animalId.Value,
                value => new AnimalId(value)
            );

        animalConfiguration.Property(a => a.Name)
            .HasColumnName("Name")
            .IsRequired()
            .HasMaxLength(100);

        animalConfiguration.Property(a => a.Age)
            .HasColumnName("Age")
            .IsRequired();

        animalConfiguration.OwnsOne(a => a.Breed, breedBuilder =>
        {
            breedBuilder.Property(b => b.Value)
                .HasMaxLength(20);
        });

        animalConfiguration.OwnsOne(a => a.AnimalType, typeBuilder =>
        {
            typeBuilder.Property(t => t.Value)
                .HasMaxLength(20);
        }).Navigation(a => a.AnimalType)
            .HasField("_type");

        animalConfiguration.Property(a => a.ImagesPath)
            .HasConversion(
                imagePath => string.Join(";", imagePath),   // de List<string> a string para DB
                imageString => imageString.Split(';', StringSplitOptions.None).ToList() // de string a List<string>
            );

        animalConfiguration.Property(a => a.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        animalConfiguration.Property(a => a.OwnerId)
            .HasColumnName("OwnerId")
            .HasConversion(
                ownerId => ownerId.Value,
                value => new OwnerId(value)
            )
            .IsRequired();
    }

}
