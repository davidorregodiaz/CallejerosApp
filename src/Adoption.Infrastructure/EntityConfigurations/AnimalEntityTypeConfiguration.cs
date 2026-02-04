using System.Text.Json;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        animalConfiguration.Property(a => a.Localization)
           .HasColumnName("Localization")
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

        var jsonOptions = new JsonSerializerOptions(); 

        animalConfiguration
            .Property(a => a.AdoptionRequirements)
            .HasConversion(
                v => JsonSerializer.Serialize(v.ToList(), jsonOptions),
                v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>())
            .HasField("_adoptionRequirements");

        animalConfiguration.Property(a => a.Sex)
            .HasConversion<string>();
        
        animalConfiguration.Property(a => a.Status)
            .HasConversion<string>();
        
        animalConfiguration.Property(a => a.Size)
            .HasConversion<string>();
        
        animalConfiguration.Property(a => a.Personality)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<Personality>>(v, (JsonSerializerOptions)null)
            )
            .HasColumnName("Personality")
            .HasField("_personality"); // <- Backing field
        
        animalConfiguration.Property(a => a.Compatibility)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<Compatibility>>(v, (JsonSerializerOptions)null)
            )
            .HasColumnName("Compatibility")
            .HasField("_compatibility"); // <- Backing field

        animalConfiguration.OwnsOne(a => a.MedicalRecord, mr =>
        {
            mr.ToTable("AnimalMedicalRecords");

            mr.Property(x => x.HealthState)
                .HasColumnName("HealthState");
            mr.Property(x => x.Vaccine)
                .HasColumnName("Vaccine");
            mr.Property(x => x.IsDewormed)
                .HasColumnName("IsDewormed");
            mr.Property(x => x.IsStirilized)
                .HasColumnName("IsStirilized");
            
            mr.WithOwner().HasForeignKey("AnimalId");
        });
    }
}
