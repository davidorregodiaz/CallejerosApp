using Adoption.API.Abstractions;
using Adoption.API.Application.Mappers;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries.AdoptionRequests;

public class GetAdoptionByIdQueryHandler(AdoptionDbContext ctx)
    : IQueryHandler<GetAdoptionRequestByIdQuery, AdoptionResponse>
{
    public async Task<Result<AdoptionResponse>> HandleAsync(GetAdoptionRequestByIdQuery query, CancellationToken cancellationToken)
    {
        var adoption = await ctx.AdoptionRequests
            .SingleOrDefaultAsync(x => x.Id == new AdoptionRequestId(query.Id), cancellationToken);
        
        if(adoption is null)
            return Result<AdoptionResponse>.FromFailure($"No adoption found with id - {query.Id.ToString()}");
        
        return Result<AdoptionResponse>.FromData(adoption.MapToResponse());
    }
}
