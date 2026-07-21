using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Domain.Interfaces.Repositories.Inventory;

public interface IInventoryMovementRepository : IRepository<InventoryMovement>
{
    Task<IReadOnlyList<InventoryMovement>> GetByProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryMovement>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default);
}
