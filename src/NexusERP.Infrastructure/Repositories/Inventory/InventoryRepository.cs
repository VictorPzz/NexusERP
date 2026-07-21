using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Inventory;

public class InventoryRepository : Repository<Domain.Entities.Inventory.Inventory>, IInventoryRepository
{
    public InventoryRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.Inventory.Inventory?> GetByProductAndWarehouseAsync(int productId, int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId, cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Inventory.Inventory>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Product)
            .Where(i => i.WarehouseId == warehouseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAvailableQuantityAsync(int productId, int warehouseId, CancellationToken cancellationToken = default)
    {
        var inventory = await GetByProductAndWarehouseAsync(productId, warehouseId, cancellationToken);
        return inventory?.Quantity ?? 0;
    }

    public async Task<bool> HasSufficientStockAsync(int productId, int warehouseId, int quantity, CancellationToken cancellationToken = default)
    {
        var available = await GetAvailableQuantityAsync(productId, warehouseId, cancellationToken);
        return available >= quantity;
    }
}
