using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.AdoptionRequests;

public class CreateAdoptionRequestAppointmentCommandHandler(
    IAdoptionRequestRepository adoptionRepository) : ICommandHandler<CreateAdoptionRequestAppointmentCommand, AppointmentViewModel>
{
    public async Task<AppointmentViewModel> HandleAsync(CreateAdoptionRequestAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        var adoption = await adoptionRepository.GetByIdAsync(command.AdoptionRequestid, cancellationToken);

        if (adoption is null)
            throw new AdoptionRequestNotFoundException($"Adoption with id {command.AdoptionRequestid} not found");

        var appointment = Appointment.Create(
            date: command.Date,
            notes: command.Notes,
            location: command.Location);

        return new AppointmentViewModel(
            AppointmentId: appointment.Id.Value,
            Status: appointment.Status,
            Date: appointment.Date,
            Notes: appointment.Notes,
            Location: appointment.Location);
    }
}
