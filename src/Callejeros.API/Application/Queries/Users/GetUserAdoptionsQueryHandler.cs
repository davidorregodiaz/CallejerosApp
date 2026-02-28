using Adoption.API.Abstractions;
using Adoption.API.Application.Exceptions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Services.Mappers;
using Adoption.Domain.AggregatesModel.AdoptionAggregate;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Shared;

namespace Adoption.API.Application.Queries.Users;

public class GetUserAdoptionsQueryHandler(IAdoptionRequestRepository adoptionRequestRepository, IUserRepository userRepository, IAdoptionMapper adoptionMapper)
    : IQueryHandler<GetUserAdoptionsQuery,PaginatedResponse<AdoptionViewModel>>
{
    public async Task<Result<PaginatedResponse<AdoptionViewModel>>> HandleAsync(GetUserAdoptionsQuery query, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.ExistsAsync(query.UserId, cancellationToken);
        
        if(!userExists)
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
            return Result<PaginatedResponse<AdoptionViewModel>>.FromFailure("No adoptions requested yet");

        var responseData = await Task.WhenAll(adoptionRequests.Select(async adoption => await adoptionMapper.MapToResponseAsync(adoption, cancellationToken)));

        var paginatedResponse = new PaginatedResponse<AdoptionViewModel>
        {
            Data = responseData, 
            TotalCount = count, 
            Page = page, 
            PageSize = pageSize
        };

        return Result<PaginatedResponse<AdoptionViewModel>>.FromData(paginatedResponse);
    }
}
