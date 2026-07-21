using NexusERP.Domain.Entities.Security;

namespace NexusERP.Domain.Interfaces.Repositories.Security;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityName, int entityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
