using Moq;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Repositories.Billing;
using NexusERP.Domain.Interfaces.Repositories.Inventory;
using NexusERP.Domain.Interfaces.Repositories.People;
using NexusERP.Domain.Interfaces.Repositories.Purchases;
using NexusERP.Domain.Interfaces.Repositories.Sales;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Tests.Fixtures;

public class MockUnitOfWorkFixture
{
    public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    public Mock<ICurrentUserService> CurrentUserServiceMock { get; } = new();
    public Mock<IInvoiceRepository> InvoiceRepositoryMock { get; } = new();
    public Mock<IPaymentRepository> PaymentRepositoryMock { get; } = new();
    public Mock<IProductRepository> ProductRepositoryMock { get; } = new();
    public Mock<IClientRepository> ClientRepositoryMock { get; } = new();
    public Mock<ISupplierRepository> SupplierRepositoryMock { get; } = new();
    public Mock<ISaleRepository> SaleRepositoryMock { get; } = new();
    public Mock<IPurchaseRepository> PurchaseRepositoryMock { get; } = new();
    public Mock<IOrderRepository> OrderRepositoryMock { get; } = new();
    public Mock<ICategoryRepository> CategoryRepositoryMock { get; } = new();
    public Mock<IWarehouseRepository> WarehouseRepositoryMock { get; } = new();

    public MockUnitOfWorkFixture()
    {
        UnitOfWorkMock.Setup(u => u.Invoices).Returns(InvoiceRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Payments).Returns(PaymentRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Products).Returns(ProductRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Clients).Returns(ClientRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Suppliers).Returns(SupplierRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Sales).Returns(SaleRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Purchases).Returns(PurchaseRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Orders).Returns(OrderRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Categories).Returns(CategoryRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.Warehouses).Returns(WarehouseRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        CurrentUserServiceMock.Setup(s => s.UserId).Returns(1);
    }
}
