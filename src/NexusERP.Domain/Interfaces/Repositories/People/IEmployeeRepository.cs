using NexusERP.Domain.Entities.People;

namespace NexusERP.Domain.Interfaces.Repositories.People;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdWithPersonAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Employee>> GetAllWithPersonAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
}
