using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Utilities;

namespace Adoption.API.Application.Queries.AdoptionRequests;

public class GetUserAdoptionsQueryHandler(IAdoptionRequestRepository adoptionRequestRepository, AdoptionDbContext  ctx)
    : IQueryHandler<GetUserAdoptionsQuery,PaginatedResponse<AdoptionResponse>>
{
    public async Task<Result<PaginatedResponse<AdoptionResponse>>> HandleAsync(GetUserAdoptionsQuery query, CancellationToken cancellationToken)
    {
        var userId = query.UserId.ToString();
        var user = await ctx.Users
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if(user is null)
            throw new UserNotFoundException($"User with id  {query.UserId} not found");
        
        var adoptions = await adoptionRequestRepository.GetAllByUserId(query.UserId, cancellationToken);

        int page = query.Page;
        int pageSize = query.PageSize;
        
        if (page < 1)
            page = 1;
        
        if(pageSize < 1)
            pageSize = 10;
        
        var adoptionRequests = adoptions.ToList();
        int count = adoptionRequests.Count();
        
        if(!adoptionRequests.Any())
            return Result<PaginatedResponse<AdoptionResponse>>.FromFailure("No adoptions requested yet");

        var responseData = adoptionRequests.Select(x => new AdoptionResponse(
            AdoptionRequestId: x.Id.Value,
            RequesterId: x.RequesterId,
            RequestDate: x.RequestDate,
            Status: x.Status,
            Comments: x.Comments));

        var paginatedResponse = new PaginatedResponse<AdoptionResponse>
        {
            Data = responseData, 
            TotalCount = count, 
            Page = query.Page, 
            PageSize = query.PageSize
        };

        return Result<PaginatedResponse<AdoptionResponse>>.FromData(paginatedResponse);
    }
}
