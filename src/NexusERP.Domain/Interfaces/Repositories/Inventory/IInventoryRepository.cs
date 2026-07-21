using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Domain.Interfaces.Repositories.Inventory;

public interface IInventoryRepository : IRepository<Entities.Inventory.Inventory>
{
    Task<Entities.Inventory.Inventory?> GetByProductAndWarehouseAsync(int productId, int warehouseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Inventory.Inventory>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default);
    Task<int> GetAvailableQuantityAsync(int productId, int warehouseId, CancellationToken cancellationToken = default);
    Task<bool> HasSufficientStockAsync(int productId, int warehouseId, int quantity, CancellationToken cancellationToken = default);
}
