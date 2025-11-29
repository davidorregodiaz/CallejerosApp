using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries.AdoptionRequests;

public class GetAdoptionByIdQueryHandler(AdoptionDbContext ctx, IAdoptionMapper adoptionMapper)
    : IQueryHandler<GetAdoptionRequestByIdQuery, AdoptionViewModel>
{
    public async Task<Result<AdoptionViewModel>> HandleAsync(GetAdoptionRequestByIdQuery query, CancellationToken cancellationToken)
    {
        var adoption = await ctx.AdoptionRequests
            .SingleOrDefaultAsync(x => x.Id == new AdoptionRequestId(query.Id), cancellationToken);
        
        if(adoption is null)
            return Result<AdoptionViewModel>.FromFailure($"No adoption found with id - {query.Id.ToString()}");
        
        return Result<AdoptionViewModel>.FromData(await adoptionMapper.MapToResponseAsync(adoption, cancellationToken));
    }
}
