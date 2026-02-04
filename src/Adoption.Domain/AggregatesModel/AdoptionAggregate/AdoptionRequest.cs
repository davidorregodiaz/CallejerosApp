using Adoption.Domain.SeedWork;
using Adoption.Domain.Events.Adoption;
using Adoption.Domain.Exceptions.Adoption;

namespace Adoption.Domain.AggregatesModel.AdoptionAggregate;

public sealed class AdoptionRequest
    : Entity, IAggregateRoot
{
    private AdoptionRequest(AdoptionRequestId id, Guid animalId, Guid requesterId, string comments) : base(id.Value)
    {
        Id = id;
        AnimalId = animalId;
        RequesterId = requesterId;
        Comments = comments;
        Status = AdoptionStatus.Pending;
        RequestDate = DateTime.UtcNow;

        AddDomainEvent(new AdoptionRequestCreatedDomainEvent(RequesterId, AnimalId, RequestDate));
    }

    public new AdoptionRequestId Id { get; private set; }
    public Guid AnimalId { get; private set; }
    public Guid RequesterId { get; private set; }
    public DateTime RequestDate { get; private set; }
    public AdoptionStatus Status { get; private set; }
    public string Comments { get; private set; }
    private List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public static AdoptionRequest Create(Guid animalId, Guid requesterId, string comments) =>
        new AdoptionRequest(new AdoptionRequestId(Guid.NewGuid()), animalId, requesterId, comments);

    public void Approve()
    {
        Status = AdoptionStatus.Approved;
        AddDomainEvent(new AdoptionStatusChangeDomainEvent(Status, RequesterId, Id.Value));
    }

    public void Reject()
    {
        Status = AdoptionStatus.Rejected;
        if (_appointments.Count != 0)
        {
            foreach (var appointment in _appointments)
            {
                appointment.Cancel();
            }
        }
        AddDomainEvent(new AdoptionStatusChangeDomainEvent(Status, RequesterId, Id.Value));
    }

    public void Complete()
    {
        Status = AdoptionStatus.Completed;
        foreach (var appointment in _appointments)
        {
            if (appointment.Status == AppointmentStatus.Scheduled)
            {
                appointment.Complete();
            }
        }
        AddDomainEvent(new AdoptionStatusChangeDomainEvent(Status, RequesterId, Id.Value));
    }

    public Appointment AddAppointment(DateTime date, string notes, string location)
    {
        if (Status != AdoptionStatus.Approved)
            throw new AdoptionDomainException("Cannot schedule appointment if adoption is not approved.");

        bool haveCurrentAppoinment = _appointments.Any(x => x.Status == AppointmentStatus.Scheduled);

        if (haveCurrentAppoinment)
            throw new AdoptionDomainException("Cannot schedule another appointment if has one scheduled already.");

        var appointment = Appointment.Create(date, notes, location);
        _appointments.Add(appointment);
        return appointment;
    }

    public void RescheduleAppointment(Guid appointmentId, DateTime dateProposed, string? rescheduleMessage)
    {
        var appointment = _appointments.SingleOrDefault(appointment => appointment.Id.Value == appointmentId);

        if (appointment is null)
            throw new AdoptionDomainException("Cannot reschedule an appointment if the appointment dont exists.");

        appointment.Reschedule(dateProposed, rescheduleMessage);
    }

    public void CancelAppointment(Guid appointmentId)
    {
        var appointment = _appointments.SingleOrDefault(x => x.Id == new AppointmentId(appointmentId));

        if (appointment is null)
            throw new AdoptionDomainException(nameof(Appointment));

        appointment.Cancel();
    }

    public void CompleteAppointment(Guid appointmentId)
    {
        var appointment = _appointments.SingleOrDefault(x => x.Id == new AppointmentId(appointmentId));

        if (appointment is null)
            throw new AdoptionDomainException(nameof(Appointment)); 
       
        appointment.Complete();
    }
    
    public void ScheduleAppointment(Guid appointmentId)
    {
        var appointment = _appointments.SingleOrDefault(x => x.Id == new AppointmentId(appointmentId));
        
        if (appointment is null)
            throw new AdoptionDomainException(nameof(Appointment));

        if (appointment.DateProposed.HasValue && appointment.DateProposed.Value > DateTime.UtcNow)
        {
            appointment.Schedule(appointment.DateProposed.Value);
        }
        else
        {
            appointment.Schedule(appointment.Date);
        }
    }

    public void RemoveAppointment(Guid appointmentId)
    {
        var appointment = _appointments.SingleOrDefault(x => x.Id == new AppointmentId(appointmentId));

        if (appointment is null)
            throw new AdoptionDomainException(nameof(Appointment));

        _appointments.Remove(appointment);
    }
}

public record AdoptionRequestId(Guid Value);
