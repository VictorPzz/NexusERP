using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Inventory;

public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.Code == code, cancellationToken);
    }

    public async Task<Warehouse?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.IsDefault, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(w => w.Code == code, cancellationToken);
    }
}
