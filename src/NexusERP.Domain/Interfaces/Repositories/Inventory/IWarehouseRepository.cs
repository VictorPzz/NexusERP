using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Domain.Interfaces.Repositories.Inventory;

public interface IWarehouseRepository : IRepository<Warehouse>
{
    Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetDefaultAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
