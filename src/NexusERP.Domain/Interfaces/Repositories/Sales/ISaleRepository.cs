using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Domain.Interfaces.Repositories.Sales;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken cancellationToken = default);
    Task<Sale?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
