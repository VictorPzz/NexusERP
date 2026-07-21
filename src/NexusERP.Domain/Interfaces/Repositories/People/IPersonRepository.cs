using NexusERP.Domain.Entities.People;

namespace NexusERP.Domain.Interfaces.Repositories.People;

public interface IPersonRepository : IRepository<Person>
{
    Task<Person?> GetByDocumentAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<bool> DocumentExistsAsync(string documentNumber, CancellationToken cancellationToken = default);
}
