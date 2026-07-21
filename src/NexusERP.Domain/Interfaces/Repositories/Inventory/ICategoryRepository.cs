using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Domain.Interfaces.Repositories.Inventory;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByIdWithChildrenAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, int? parentId, CancellationToken cancellationToken = default);
}
