using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Inventory;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByIdWithChildrenAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.ParentId == null)
            .Include(c => c.Children)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, int? parentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(c => c.Name == name && c.ParentId == parentId, cancellationToken);
    }
}
