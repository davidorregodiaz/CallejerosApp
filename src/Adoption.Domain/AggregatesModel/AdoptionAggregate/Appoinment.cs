using System.Text.Json.Serialization;
using Adoption.Domain.Exceptions.Adoption;
using Adoption.Domain.SeedWork;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

public class Appointment : Entity
{
    public new AppointmentId Id { get; private set; }
    public DateTime Date { get; private set; }
    public string Notes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string Location { get; private set; }
    public AdoptionRequestId AdoptionRequestId { get; private set; }
    private Appointment() : base(Guid.Empty)//EF
    {
        Id = new AppointmentId(Guid.Empty);
    } 
    private Appointment(AppointmentId id, DateTime date, string notes, string location) : base(id.Value)
    {
        Id = id;
        Date = date;
        Notes = notes;
        Status = AppointmentStatus.Pending;
        Location = location;
    }

    public static Appointment Create(DateTime date, string notes, string location)
    {
        return new Appointment(
            new AppointmentId(Guid.NewGuid()),
            date,
            notes,
            location);
    }

    public void Reschedule(DateTime newDate)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new AdoptionDomainException("Cannot reschedule a cancelled appointment.");

        Date = newDate;
    }

    public void Schedule(DateTime newDate)
    {
        if (Status == AppointmentStatus.Pending)
        {
            Date = newDate;
        }
        else
        {
            throw new AdoptionDomainException("Cannot schedule a cancelled, completed or already scheduled appointment.");
        }
    }

    public void Cancel()
    {
        Status = AppointmentStatus.Cancelled;
    }
}

public record AppointmentId(Guid Value);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppointmentStatus
{
    Pending,
    Scheduled,
    Completed,
    Cancelled
}
