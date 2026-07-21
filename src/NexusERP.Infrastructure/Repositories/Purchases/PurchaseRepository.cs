using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Purchases;
using NexusERP.Domain.Interfaces.Repositories.Purchases;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Purchases;

public class PurchaseRepository : Repository<Purchase>, IPurchaseRepository
{
    public PurchaseRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Purchase?> GetByNumberAsync(string purchaseNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.PurchaseNumber == purchaseNumber, cancellationToken);
    }

    public async Task<Purchase?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Supplier)
                .ThenInclude(s => s.Person)
            .Include(p => p.Details)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
