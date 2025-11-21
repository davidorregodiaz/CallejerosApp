using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;

namespace Adoption.API.Application.Mappers;

public static class AdoptionRequestMapper
{
    public static AdoptionResponse MapToResponse(this AdoptionRequest adoptionRequest)
    {
        return new AdoptionResponse(
            AdoptionRequestId: adoptionRequest.Id.Value,
            RequesterId: adoptionRequest.RequesterId,
            RequestDate: adoptionRequest.RequestDate,
            Status: adoptionRequest.Status,
            Comments: adoptionRequest.Comments);
    }
}
