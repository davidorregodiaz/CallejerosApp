using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public record CreateAdoptionRequestAppointmentCommand(
    Guid AdoptionRequestid,
    DateTime Date,
    string Notes,
    string Location) : ICommand<AppointmentViewModel>;
