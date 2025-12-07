namespace Adoption.Domain.AggregatesModel.UserAggregate;

public interface IUserRepository
{
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}
