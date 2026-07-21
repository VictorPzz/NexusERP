using FluentAssertions;
using Moq;
using NexusERP.Application.Modules.Billing.Queries.GetInvoiceById;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Entities.Billing;
using NexusERP.Domain.Entities.People;
using NexusERP.Tests.Fixtures;

namespace NexusERP.Tests.Queries.Billing;

public class GetInvoiceByIdQueryHandlerTests : IDisposable
{
    private readonly MockUnitOfWorkFixture _fixture = new();
    private readonly GetInvoiceByIdQueryHandler _handler;

    public GetInvoiceByIdQueryHandlerTests()
    {
        _handler = new GetInvoiceByIdQueryHandler(_fixture.UnitOfWorkMock.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task Should_Return_Invoice_When_Found()
    {
        var invoice = new Invoice
        {
            Id = 1,
            InvoiceNumber = "FACB-20260721-0001",
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Client = new Client { Id = 1, Person = new Person { Id = 1, FirstName = "Juan", LastName = "Perez" } },
            InvoiceDate = DateTime.UtcNow,
            Status = InvoiceStatus.issued,
            Subtotal = 1000,
            TaxAmount = 160,
            Total = 1160,
            AmountPaid = 0,
            BalanceDue = 1160,
            Details = new List<InvoiceDetail>(),
            Payments = new List<Payment>()
        };

        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

        var result = await _handler.Handle(new GetInvoiceByIdQuery { Id = 1 }, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.InvoiceNumber.Should().Be("FACB-20260721-0001");
        result.Data.ClientName.Should().Be("Juan Perez");
        result.Data.Total.Should().Be(1160);
        result.Data.BalanceDue.Should().Be(1160);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Invoice_Does_Not_Exist()
    {
        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _handler.Handle(new GetInvoiceByIdQuery { Id = 999 }, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task Should_Map_Supplier_Name_Correctly()
    {
        var invoice = new Invoice
        {
            Id = 2,
            InvoiceNumber = "FACB-20260721-0002",
            InvoiceType = InvoiceType.purchase,
            SupplierId = 1,
            Supplier = new Supplier { Id = 1, Person = new Person { Id = 2, FirstName = "Proveedor", LastName = "ABC" } },
            InvoiceDate = DateTime.UtcNow,
            Status = InvoiceStatus.issued,
            Subtotal = 500,
            TaxAmount = 80,
            Total = 580,
            Details = new List<InvoiceDetail>(),
            Payments = new List<Payment>()
        };

        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

        var result = await _handler.Handle(new GetInvoiceByIdQuery { Id = 2 }, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.SupplierName.Should().Be("Proveedor ABC");
        result.Data.ClientName.Should().BeNull();
    }

    [Fact]
    public async Task Should_Map_Payment_Count_Correctly()
    {
        var invoice = new Invoice
        {
            Id = 1,
            InvoiceNumber = "FACB-20260721-0001",
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            InvoiceDate = DateTime.UtcNow,
            Status = InvoiceStatus.issued,
            Subtotal = 1000,
            TaxAmount = 0,
            Total = 1000,
            AmountPaid = 500,
            BalanceDue = 500,
            Details = new List<InvoiceDetail>
            {
                new() { Id = 1, ProductId = 1, Quantity = 2, UnitPrice = 500, Subtotal = 1000, Total = 1000 }
            },
            Payments = new List<Payment>
            {
                new() { Id = 1, Amount = 500, PaymentMethod = PaymentMethod.cash }
            }
        };

        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

        var result = await _handler.Handle(new GetInvoiceByIdQuery { Id = 1 }, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.ItemCount.Should().Be(1);
        result.Data.PaymentCount.Should().Be(1);
    }
}
