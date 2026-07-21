using NexusERP.Domain.Entities.People;

namespace NexusERP.Domain.Interfaces.Repositories.People;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Client?> GetByIdWithPersonAsync(int id, CancellationToken cancellationToken = default);
    Task<Client?> GetByIdWithAddressesAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Client>> GetAllWithPersonAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
