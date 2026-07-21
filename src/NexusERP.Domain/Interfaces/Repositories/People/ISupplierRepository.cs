using NexusERP.Domain.Entities.People;

namespace NexusERP.Domain.Interfaces.Repositories.People;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdWithPersonAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Supplier>> GetAllWithPersonAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
