using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.API.Extensions;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries.AdoptionRequests;

public class GetAllAdoptionsQueryHandler(
    AdoptionDbContext ctx, 
    IAnimalRepository  animalRepository,
    IAdoptionMapper adoptionMapper, 
    UserManager<ApplicationUser> userManager,
    ILogger<GetAllAdoptionsQueryHandler> logger)
    : IQueryHandler<GetAllAdoptionsQuery, PaginatedResponse<AdoptionViewModel>>
{
    public async Task<Result<PaginatedResponse<AdoptionViewModel>>> HandleAsync(GetAllAdoptionsQuery query, CancellationToken cancellationToken)
    {
        var queryable = ctx.AdoptionRequests
            .Include(a => a.Appointments)
            .AsQueryable();
        
        if(!queryable.Any())
            return Result<PaginatedResponse<AdoptionViewModel>>.FromFailure("No adoption requests found");
            
        int page = query.Page;
        int pageSize = query.PageSize;
        
        if (page < 1)
            page = 1;
        
        if(pageSize < 1)
            pageSize = 10;

        //Filtrado por ID de Usuario y su Rol
        if (query.UserId.HasValue)
        {
            var user = await userManager.FindByIdAsync(query.UserId.ToString()!);
            
            if(user == null)
                return Result<PaginatedResponse<AdoptionViewModel>>.FromFailure("User not found");

            if (await userManager.IsInRoleAsync(user, Roles.OWNER))
            {
                var animals = await animalRepository
                    .GetAnimalsByUserId(Guid.TryParse(user.Id, out var userId) ? userId : Guid.Empty, cancellationToken);

                var animalsIds = animals.Select(a => a.Id.Value).ToList();

                queryable = queryable.Where(adoption => animalsIds.Contains(adoption.AnimalId));
            }
            else
            {
                queryable = queryable.Where(adoption => adoption.RequesterId == query.UserId);
            }
        }
        
        if(query.Date.HasValue)
            queryable = queryable.Where(x => x.RequestDate == query.Date);
        
        if(query.Status.HasValue)
            queryable = queryable.Where(x => x.Status == query.Status);
        
        int totalCount = queryable.Count();
        var adoptions = (await queryable.ToListAsync(cancellationToken)).AsEnumerable();
        
        adoptions = adoptions.OrderByProperty(query.SortBy, query.IsDescending)
            .PaginatePage(page, pageSize);
        
        var responseData = new List<AdoptionViewModel>();

        foreach (var adoption in adoptions)
        {
            responseData.Add(await adoptionMapper.MapToResponseAsync(adoption, cancellationToken));
        }
        
        var paginatedResponse = new PaginatedResponse<AdoptionViewModel>
        {
            Data = responseData,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
        
        return Result<PaginatedResponse<AdoptionViewModel>>.FromData(paginatedResponse);
    }
}
