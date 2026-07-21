using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Domain.Interfaces.Repositories.Sales;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
