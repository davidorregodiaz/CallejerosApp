using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Commands.Appointments;

public record CreateAppointmentCommand(
    Guid AdoptionRequestid,
    DateTime Date,
    string Notes,
    string Location) : ICommand<AppointmentViewModel>;
