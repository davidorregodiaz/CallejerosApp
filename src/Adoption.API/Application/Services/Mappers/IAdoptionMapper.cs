using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Services.Mappers;

public interface IAdoptionMapper
{
    Task<AdoptionViewModel> MapToResponseAsync(AdoptionRequest adoptionRequest, CancellationToken cancellationToken);
}
