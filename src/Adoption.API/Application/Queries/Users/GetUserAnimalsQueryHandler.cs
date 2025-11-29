using Adoption.API.Abstractions;
using Adoption.API.Application.Commands.Animals;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.API.Application.Services.Minio;
using Adoption.Domain.AggregatesModel.AnimalAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Shared;

namespace Adoption.API.Application.Queries.Users;

public class GetUserAnimalsQueryHandler(IAnimalRepository animalRepository, IUserRepository userRepository, IAnimalMapper animalMapper)
    : IQueryHandler<GetUserAnimalsQuery, PaginatedResponse<AnimalViewModel>>
{
    public async Task<Result<PaginatedResponse<AnimalViewModel>>> HandleAsync(GetUserAnimalsQuery query, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.ExistsAsync(query.UserId, cancellationToken);
        
        if(!userExists)
            throw new UserNotFoundException($"User with id  {query.UserId} not found");
        
        var animals = await animalRepository.GetAnimalsByUserId(query.UserId, cancellationToken);

        animals = animals.ToList();
        
        if(!animals.Any())
            return Result<PaginatedResponse<AnimalViewModel>>.FromFailure("No adoptions requested yet");
        
        int page = query.Page;
        int pageSize = query.PageSize;
        
        if (page < 1)
            page = 1;
        
        if(pageSize < 1)
            pageSize = 10;
        
        int count = animals.Count();

        var responseData = await Task.WhenAll(
            animals.Select(async animal => await animalMapper.MapToResponse(animal,cancellationToken)));

        var paginatedResponse = new PaginatedResponse<AnimalViewModel>
        {
            Data = responseData, 
            TotalCount = count, 
            Page = page, 
            PageSize = pageSize
        };

        return Result<PaginatedResponse<AnimalViewModel>>.FromData(paginatedResponse);
    }
}
