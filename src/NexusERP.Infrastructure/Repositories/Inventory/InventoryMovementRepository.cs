using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Inventory;

public class InventoryMovementRepository : Repository<InventoryMovement>, IInventoryMovementRepository
{
    public InventoryMovementRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<InventoryMovement>> GetByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.ProductId == productId)
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryMovement>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.WarehouseId == warehouseId)
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync(cancellationToken);
    }
}
