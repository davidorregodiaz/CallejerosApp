using Adoption.API.Application.Models.User;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Models;

public record AdoptionViewModel(
    Guid AdoptionRequestId,
    AnimalViewModel Animal,
    UserViewModel Requester,
    DateTime RequestDate,
    AdoptionStatus Status,
    string Comments,
    IReadOnlyCollection<AppointmentViewModel> Appointments);
