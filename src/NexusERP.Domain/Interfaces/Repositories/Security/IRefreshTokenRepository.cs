using NexusERP.Domain.Entities.Security;

namespace NexusERP.Domain.Interfaces.Repositories.Security;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveTokensByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default);
}
