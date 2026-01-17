using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Commands.Appointments;

public class CreateAppointmentCommandHandler(
    IAdoptionRequestRepository adoptionRepository,
    IAdoptionMapper adoptionMapper) : ICommandHandler<CreateAppointmentCommand, AppointmentViewModel>
{
    public async Task<AppointmentViewModel> HandleAsync(CreateAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        var adoption = await adoptionRepository.GetByIdAsync(command.AdoptionRequestid, cancellationToken);

        if (adoption is null)
            throw new AdoptionRequestNotFoundException($"Adoption with id {command.AdoptionRequestid} not found");
        
        var appointment = adoption.AddAppointment(date: command.Date, notes: command.Notes, location: command.Location);

        await adoptionRepository.UnitOfWork().SaveChangesAsync(cancellationToken);

        return appointment.ToResponse();
    }
}
