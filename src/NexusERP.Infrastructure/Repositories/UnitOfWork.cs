using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Repositories.Security;
using NexusERP.Domain.Interfaces.Repositories.People;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Domain.Interfaces.Repositories.Purchases;
using NexusERP.Domain.Interfaces.Repositories.Sales;
using NexusERP.Domain.Interfaces.Repositories.Billing;
using NexusERP.Infrastructure.Persistence;
using NexusERP.Infrastructure.Repositories.Security;
using NexusERP.Infrastructure.Repositories.People;
using NexusERP.Infrastructure.Repositories.Inventory;
using NexusERP.Infrastructure.Repositories.Purchases;
using NexusERP.Infrastructure.Repositories.Sales;
using NexusERP.Infrastructure.Repositories.Billing;

namespace NexusERP.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly NexusERPDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(NexusERPDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Roles = new RoleRepository(context);
        RefreshTokens = new RefreshTokenRepository(context);
        AuditLogs = new AuditLogRepository(context);
        Persons = new PersonRepository(context);
        Employees = new EmployeeRepository(context);
        Clients = new ClientRepository(context);
        Suppliers = new SupplierRepository(context);
        Products = new ProductRepository(context);
        Categories = new CategoryRepository(context);
        Inventories = new InventoryRepository(context);
        Warehouses = new WarehouseRepository(context);
        InventoryMovements = new InventoryMovementRepository(context);
        PurchaseOrders = new PurchaseOrderRepository(context);
        Purchases = new PurchaseRepository(context);
        Orders = new OrderRepository(context);
        Sales = new SaleRepository(context);
        Invoices = new InvoiceRepository(context);
        Payments = new PaymentRepository(context);
    }

    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IAuditLogRepository AuditLogs { get; }
    public IPersonRepository Persons { get; }
    public IEmployeeRepository Employees { get; }
    public IClientRepository Clients { get; }
    public ISupplierRepository Suppliers { get; }
    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public IInventoryRepository Inventories { get; }
    public IWarehouseRepository Warehouses { get; }
    public IInventoryMovementRepository InventoryMovements { get; }
    public IPurchaseOrderRepository PurchaseOrders { get; }
    public IPurchaseRepository Purchases { get; }
    public IOrderRepository Orders { get; }
    public ISaleRepository Sales { get; }
    public IInvoiceRepository Invoices { get; }
    public IPaymentRepository Payments { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
