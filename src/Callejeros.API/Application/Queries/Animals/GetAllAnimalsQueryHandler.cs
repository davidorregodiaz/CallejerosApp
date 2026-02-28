using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.API.Extensions;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries.Animals;

public class GetAllAnimalsQueryHandler(AdoptionDbContext ctx, IAnimalMapper animalMapper)
    : IQueryHandler<GetAllAnimalsQuery, PaginatedResponse<AnimalViewModel>>
{
    public async Task<Result<PaginatedResponse<AnimalViewModel>>> HandleAsync(GetAllAnimalsQuery query, CancellationToken cancellationToken)
    {
        var queryable = ctx.Animals
            .Where(a => a.Status != AnimalStatus.Adopted || a.Status == AnimalStatus.Hide)
            .AsNoTracking().AsQueryable();

        if (!queryable.Any())
            return Result<PaginatedResponse<AnimalViewModel>>.FromFailure("No animals available");
        
        int page = query.Page;
        int pageSize = query.PageSize;
        
        if (page < 1)
            page = 1;
        
        if(pageSize < 1)
            pageSize = 10;

        if (!string.IsNullOrEmpty(query.Name))
        {
            string normalizedName = query.Name.Trim().ToLower();
            queryable = queryable.Where(x => x.NormalizedName.Contains(normalizedName));
        }

        if (!string.IsNullOrEmpty(query.Localization))
        {
            string normalizedLocalization = query.Localization.Trim().ToLower();
            queryable = queryable.Where(x => x.NormalizedLocalization.Contains(normalizedLocalization));
        }

        if (!string.IsNullOrEmpty(query.Species))
        {
            string normalizedSpecies = query.Species.Trim().ToLower();
            queryable = queryable.Where(x => x.NormalizedSpecies.Contains(normalizedSpecies));
        }

        if (query.Age != null && query.Age > 0)
            queryable = queryable.Where(x => x.Age == query.Age);

        if (query.OwnerId != null)
            queryable = queryable.Where(x => x.OwnerId == new OwnerId(query.OwnerId.Value));

        int totalCount = queryable.Count();
        var animals = (await queryable.ToListAsync(cancellationToken)).AsEnumerable();
        
        animals = animals.OrderByProperty(query.SortBy, query.IsDescending)
            .PaginatePage(page, pageSize);

        var paginatedResponse = new PaginatedResponse<AnimalViewModel>
        {
            Data = await Task.WhenAll(
                animals.Select(async animal => await animalMapper.MapToResponse(animal, cancellationToken))),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
        
        return Result<PaginatedResponse<AnimalViewModel>>.FromData(paginatedResponse);
    }
}
