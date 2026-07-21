using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Sales;
using NexusERP.Domain.Interfaces.Repositories.Sales;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Sales;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Client)
                .ThenInclude(c => c!.Person)
            .Include(o => o.Supplier)
                .ThenInclude(s => s!.Person)
            .Include(o => o.Details)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
