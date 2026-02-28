using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AnimalAggregate;

namespace Adoption.API.Application.Services.Mappers;

public interface IAnimalMapper
{
    Task<AnimalViewModel> MapToResponse(Animal animal, CancellationToken ct);
}
