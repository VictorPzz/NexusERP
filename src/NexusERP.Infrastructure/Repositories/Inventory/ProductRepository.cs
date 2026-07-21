using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Inventory;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<bool> BarcodeExistsAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(p => p.Barcode == barcode, cancellationToken);
    }

    public async Task<List<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .ToListAsync(cancellationToken);
    }
}
