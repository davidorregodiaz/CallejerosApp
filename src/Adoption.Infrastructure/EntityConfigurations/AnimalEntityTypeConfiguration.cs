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

        animalConfiguration.Property(a => a.Breed)
           .HasColumnName("Breed")
           .IsRequired()
           .HasMaxLength(50);

        animalConfiguration.Property(a => a.Species)
           .HasColumnName("Species")
           .IsRequired()
           .HasMaxLength(50);

        animalConfiguration.Property(a => a.AdditionalImagesUrl)
            .HasConversion(
                imagePath => (imagePath == null || imagePath.Count == 0) ? null : string.Join(";", imagePath), 
                imageString => string.IsNullOrEmpty(imageString) ? new List<string>() : imageString.Split(';', StringSplitOptions.None).ToList() 
            )
            .HasField("_aditionalImages"); ;

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
