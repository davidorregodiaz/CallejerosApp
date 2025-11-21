using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Extensions;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries.AdoptionRequests;

public class GetAllAdoptionsQueryHandler(AdoptionDbContext ctx)
    : IQueryHandler<GetAllAdoptionsQuery, PaginatedResponse<AdoptionResponse>>
{
    public async Task<Result<PaginatedResponse<AdoptionResponse>>> HandleAsync(GetAllAdoptionsQuery query, CancellationToken cancellationToken)
    {
        var queryable = ctx.AdoptionRequests.AsQueryable();
        
        if(!queryable.Any())
            return Result<PaginatedResponse<AdoptionResponse>>.FromFailure("No adoption requests found");
            
        int page = query.Page;
        int pageSize = query.PageSize;
        
        if (page < 1)
            page = 1;
        
        if(pageSize < 1)
            pageSize = 10;
        
        if(query.RequesterId.HasValue)
            queryable = queryable.Where(x => x.RequesterId == query.RequesterId);
        
        if(query.Date.HasValue)
            queryable = queryable.Where(x => x.RequestDate == query.Date);
        
        if(query.Status.HasValue)
            queryable = queryable.Where(x => x.Status == query.Status);
        
        int totalCount = queryable.Count();
        var adoptions = (await queryable.ToListAsync(cancellationToken)).AsEnumerable();
        
        adoptions = adoptions.OrderByProperty(query.SortBy, query.IsDescending)
            .PaginatePage(page, pageSize);
        
        var paginatedResponse = new PaginatedResponse<AdoptionResponse>
        {
            Data = adoptions.Select(x => new AdoptionResponse(
                AdoptionRequestId: x.Id.Value,
                RequesterId: x.RequesterId,
                RequestDate: x.RequestDate,
                Status: x.Status,
                Comments: x.Comments)),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
        
        return Result<PaginatedResponse<AdoptionResponse>>.FromData(paginatedResponse);
    }
}
