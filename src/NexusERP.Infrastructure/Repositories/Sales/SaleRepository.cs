using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Sales;
using NexusERP.Domain.Interfaces.Repositories.Sales;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Sales;

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Sale?> GetByNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<Sale?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Client)
                .ThenInclude(c => c.Person)
            .Include(s => s.Details)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
