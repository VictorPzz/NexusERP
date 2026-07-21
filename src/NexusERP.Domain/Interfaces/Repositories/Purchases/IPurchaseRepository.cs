using NexusERP.Domain.Entities.Purchases;

namespace NexusERP.Domain.Interfaces.Repositories.Purchases;

public interface IPurchaseRepository : IRepository<Purchase>
{
    Task<Purchase?> GetByNumberAsync(string purchaseNumber, CancellationToken cancellationToken = default);
    Task<Purchase?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
