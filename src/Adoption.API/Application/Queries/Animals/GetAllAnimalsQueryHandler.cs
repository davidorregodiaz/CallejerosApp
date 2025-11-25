using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Extensions;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Utilities;

namespace Adoption.API.Application.Queries.Animals;

public class GetAllAnimalsQueryHandler(AdoptionDbContext ctx)
    : IQueryHandler<GetAllAnimalsQuery, PaginatedResponse<AnimalResponse>>
{
    public async Task<Result<PaginatedResponse<AnimalResponse>>> HandleAsync(GetAllAnimalsQuery query, CancellationToken cancellationToken)
    {
        var queryable = ctx.Animals.AsNoTracking().AsQueryable();

        if (!queryable.Any())
            return Result<PaginatedResponse<AnimalResponse>>.FromFailure("No animals available");
        
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

        if (!string.IsNullOrEmpty(query.Breed))
        {
            string normalizedBreed = query.Breed.Trim().ToLower();
            queryable = queryable.Where(x => x.NormalizedBreed.Contains(normalizedBreed));
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
        

        var paginatedResponse = new PaginatedResponse<AnimalResponse>
        {
            Data = animals.Select(x => new AnimalResponse(
                Id: x.Id.Value,
                OwnerId: x.OwnerId.Value,
                Name: x.Name,
                Species: x.Species,
                Breed: x.Breed,
                Age : x.Age,
                Description: x.Description,
                PrincipalImageUrl: x.PrincipalImage,
                ExtraImagesUrls: x.AdditionalImagesUrl?.ToList()
            )),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
        
        return Result<PaginatedResponse<AnimalResponse>>.FromData(paginatedResponse);
    }
}
