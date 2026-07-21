using NexusERP.Domain.Interfaces.Repositories.Security;
using NexusERP.Domain.Interfaces.Repositories.People;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Domain.Interfaces.Repositories.Purchases;
using NexusERP.Domain.Interfaces.Repositories.Sales;
using NexusERP.Domain.Interfaces.Repositories.Billing;

namespace NexusERP.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IAuditLogRepository AuditLogs { get; }
    IPersonRepository Persons { get; }
    IEmployeeRepository Employees { get; }
    IClientRepository Clients { get; }
    ISupplierRepository Suppliers { get; }
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IInventoryRepository Inventories { get; }
    IWarehouseRepository Warehouses { get; }
    IInventoryMovementRepository InventoryMovements { get; }
    IPurchaseOrderRepository PurchaseOrders { get; }
    IPurchaseRepository Purchases { get; }
    IOrderRepository Orders { get; }
    ISaleRepository Sales { get; }
    IInvoiceRepository Invoices { get; }
    IPaymentRepository Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
