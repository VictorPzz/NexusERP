using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Security;
using NexusERP.Domain.Interfaces.Repositories.Security;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Security;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(r => r.Name == name, cancellationToken);
    }
}
