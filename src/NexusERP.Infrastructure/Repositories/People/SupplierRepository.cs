using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.People;
using NexusERP.Domain.Interfaces.Repositories.People;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.People;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.SupplierCode == code, cancellationToken);
    }

    public async Task<Supplier?> GetByIdWithPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(s => s.SupplierCode == code, cancellationToken);
    }

    public async Task<List<Supplier>> GetAllWithPersonAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Person)
            .ToListAsync(cancellationToken);
    }
}
