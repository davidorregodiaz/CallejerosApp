
using Adoption.API.Abstractions;
using Adoption.API.Application.Models;
using Adoption.API.Application.Models.User;
using Adoption.API.Application.Services.Minio;
using Adoption.API.Extensions;
using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Adoption.API.Application.Queries.Users;

public class GetAllUsersQueryHandler(AdoptionDbContext ctx, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMinioService minioService)
    : IQueryHandler<GetAllUsersQuery, PaginatedResponse<UserViewModel>>
{
    public async Task<Result<PaginatedResponse<UserViewModel>>> HandleAsync(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var queryable = ctx.Users.AsNoTracking().AsQueryable();

        if (!queryable.Any())
            return Result<PaginatedResponse<UserViewModel>>.FromFailure("No users register");

        int page = query.Page;
        int pageSize = query.PageSize;

        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        int totalCount = queryable.Count();
        var users = (await queryable.ToListAsync(cancellationToken)).AsEnumerable();

        users = users.OrderByProperty(query.SortBy, query.IsDescending)
            .PaginatePage(page, pageSize);

        var userResponses = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = (await userManager.GetRolesAsync(user)).ToList();
            var imageUrl = await minioService.PresignedGetUrl(user.AvatarUrl ?? string.Empty, cancellationToken);
            userResponses.Add(new UserViewModel(
                Id: Guid.TryParse(user.Id, out var result) ? result : Guid.Empty,
                Username: user.UserName!,
                Email: user.Email!,
                JoinedAt: user.JoinedAt,
                ImageUrl: imageUrl,
                Roles: roles));
        }
        
        var paginatedResponse = new PaginatedResponse<UserViewModel>
        {
            Data = userResponses,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };

        return Result<PaginatedResponse<UserViewModel>>.FromData(paginatedResponse);
    }
}
