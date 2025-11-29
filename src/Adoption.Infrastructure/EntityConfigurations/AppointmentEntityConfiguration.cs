using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adoption.Infrastructure.EntityConfigurations;

public class AppointmentEntityConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        
        builder.Ignore(a => a.DomainEvents);

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(
                appointmentId => appointmentId.Value,
                value => new AppointmentId(value)
            );

        builder.Property(a => a.Date)
            .IsRequired();

        builder.Property(a => a.Notes)
            .HasMaxLength(500);

        builder.Property(a => a.Status)
            .IsRequired();

        builder.HasOne<AdoptionRequest>()
            .WithMany(a => a.Appointments)
            .HasForeignKey(a => a.AdoptionRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
