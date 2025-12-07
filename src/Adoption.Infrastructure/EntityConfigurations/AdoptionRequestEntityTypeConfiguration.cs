
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Adoption.Domain.AggregatesModel.AdoptionAggregate.AdoptionRequest;

namespace Adoption.Infrastructure.EntityConfigurations;

public class AdoptionRequestEntityTypeConfiguration
    : IEntityTypeConfiguration<AdoptionRequest>
{
    public void Configure(EntityTypeBuilder<AdoptionRequest> adoptionRequestConfiguration)
    {
        adoptionRequestConfiguration.ToTable("AdoptionRequests");

        adoptionRequestConfiguration.Ignore(ar => ar.DomainEvents);

        adoptionRequestConfiguration.HasKey(ar => ar.Id);

        adoptionRequestConfiguration.Property(ar => ar.Id)
            .HasConversion(
                id => id.Value,
                value => new AdoptionRequestId(value)
            );

        adoptionRequestConfiguration.Property(ar => ar.AnimalId)
            .HasColumnName("AnimalId")
            .IsRequired();

        adoptionRequestConfiguration.Property(ar => ar.RequesterId)
            .HasColumnName("RequesterId")
            .IsRequired();

        adoptionRequestConfiguration.Property(ar => ar.RequestDate)
            .HasColumnName("RequestDate")
            .IsRequired();

        adoptionRequestConfiguration.Property(ar => ar.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        adoptionRequestConfiguration.Property(ar => ar.Comments)
            .HasColumnName("Comments")
            .HasMaxLength(500);
        
        adoptionRequestConfiguration.Metadata
            .FindNavigation(nameof(AdoptionRequest.Appointments))
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
