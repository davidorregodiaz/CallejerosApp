using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Queries.Animals;

public record GetAnimalByIdQuery(Guid Id) : IQuery<AnimalViewModel>;
