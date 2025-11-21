using Adoption.API.Abstractions;
using Adoption.API.Application.Models;

namespace Adoption.API.Application.Queries.AdoptionRequests;

public record GetAdoptionRequestByIdQuery(
    Guid Id) : IQuery<AdoptionResponse>;
