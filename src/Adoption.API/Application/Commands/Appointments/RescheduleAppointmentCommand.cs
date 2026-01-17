using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Commands.Appointments;

public record RescheduleAppointmentCommand(
    Guid AppointmentId,
    DateTime DateProposed,
    string? RescheduleMessage): ICommand<AppointmentViewModel>;
