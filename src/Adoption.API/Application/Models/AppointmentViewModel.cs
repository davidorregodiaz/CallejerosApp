using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Models;

public record AppointmentViewModel(
    Guid AppointmentId,
    AppointmentStatus Status,
    DateTime Date,
    string Notes,
    string Location,
    DateTime? DateProposed,
    string? RescheduleMessage);
