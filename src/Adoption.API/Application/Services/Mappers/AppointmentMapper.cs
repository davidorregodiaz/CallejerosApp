using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Services.Mappers;

public static class AppointmentMapper
{
    public static AppointmentViewModel ToResponse(this Appointment appointment)
    {
        return new AppointmentViewModel(
            AppointmentId: appointment.Id.Value,
            Status: appointment.Status,
            Date: appointment.Date,
            Notes: appointment.Notes,
            Location: appointment.Location,
            DateProposed: appointment.DateProposed,
            RescheduleMessage: appointment.RescheduleMessage);
    }
}
