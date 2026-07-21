using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Purchases;
using NexusERP.Domain.Interfaces.Repositories.Purchases;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Purchases;

public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<PurchaseOrder?> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<PurchaseOrder?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Supplier)
                .ThenInclude(s => s.Person)
            .Include(o => o.Details)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
