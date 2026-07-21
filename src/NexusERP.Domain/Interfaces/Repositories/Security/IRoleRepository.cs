using NexusERP.Domain.Entities.Security;

namespace NexusERP.Domain.Interfaces.Repositories.Security;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
}
