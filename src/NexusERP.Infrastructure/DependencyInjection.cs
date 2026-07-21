using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Repositories.Security;
using NexusERP.Domain.Interfaces.Repositories.People;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Domain.Interfaces.Repositories.Purchases;
using NexusERP.Domain.Interfaces.Repositories.Sales;
using NexusERP.Domain.Interfaces.Repositories.Billing;
using NexusERP.Domain.Interfaces.Services;
using NexusERP.Infrastructure.Persistence;
using NexusERP.Infrastructure.Repositories;
using NexusERP.Infrastructure.Repositories.Security;
using NexusERP.Infrastructure.Repositories.People;
using NexusERP.Infrastructure.Repositories.Inventory;
using NexusERP.Infrastructure.Repositories.Purchases;
using NexusERP.Infrastructure.Repositories.Sales;
using NexusERP.Infrastructure.Repositories.Billing;
using NexusERP.Infrastructure.Services;

namespace NexusERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NexusERPDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();

        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();

        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
