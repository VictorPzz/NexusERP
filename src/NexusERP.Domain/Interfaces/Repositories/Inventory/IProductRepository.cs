using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Domain.Interfaces.Repositories.Inventory;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> BarcodeExistsAsync(string barcode, CancellationToken cancellationToken = default);
}
