using Adoption.Domain.AggregatesModel.UserAggregate;
using Adoption.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Adoption.Infrastructure.Repositories;

public class UserRepository(AdoptionDbContext ctx) : IUserRepository
{
    public Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken) =>
        ctx.Users.AnyAsync(x => x.Id == userId.ToString(), cancellationToken);
    
    public Task<ApplicationUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken) =>
        ctx.Users.SingleOrDefaultAsync(x => x.Id == userId.ToString(), cancellationToken);
}
