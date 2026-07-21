using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Security;
using NexusERP.Domain.Interfaces.Repositories.Security;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Security;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityName, int entityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.EntityName == entityName && a.EntityId == entityId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
