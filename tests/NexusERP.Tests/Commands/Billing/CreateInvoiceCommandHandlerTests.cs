using FluentAssertions;
using Moq;
using NexusERP.Application.Modules.Billing.Commands.CreateInvoice;
using NexusERP.Application.Modules.Billing.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Entities.People;
using NexusERP.Tests.Fixtures;

namespace NexusERP.Tests.Commands.Billing;

public class CreateInvoiceCommandHandlerTests : IDisposable
{
    private readonly MockUnitOfWorkFixture _fixture = new();
    private readonly CreateInvoiceCommandHandler _handler;

    public CreateInvoiceCommandHandlerTests()
    {
        _handler = new CreateInvoiceCommandHandler(
            _fixture.UnitOfWorkMock.Object,
            _fixture.CurrentUserServiceMock.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task Should_Create_Invoice_Successfully()
    {
        var product = new Product { Id = 1, Name = "Laptop", CostPrice = 800, SellingPrice = 1200 };
        _fixture.ProductRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _fixture.InvoiceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Billing.Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Billing.Invoice inv, CancellationToken _) => { inv.Id = 1; return inv; });
        _fixture.InvoiceRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Billing.Invoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Notes = "Test invoice",
            Details = new List<InvoiceDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 1200, TaxRate = 16 }
            }
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().BeGreaterThan(0);
        _fixture.InvoiceRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Billing.Invoice>(), It.IsAny<CancellationToken>()), Times.Once);
        _fixture.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Return_Fail_When_Product_Not_Found()
    {
        _fixture.ProductRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Details = new List<InvoiceDetailCreateDto>
            {
                new() { ProductId = 999, Quantity = 1, UnitPrice = 100 }
            }
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task Should_Calculate_Totals_Correctly()
    {
        var product = new Product { Id = 1, Name = "Widget", CostPrice = 10, SellingPrice = 20 };
        Domain.Entities.Billing.Invoice? captured = null;
        _fixture.ProductRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _fixture.InvoiceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Billing.Invoice>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Billing.Invoice, CancellationToken>((inv, _) => captured = inv)
            .ReturnsAsync((Domain.Entities.Billing.Invoice inv, CancellationToken _) => inv);
        _fixture.InvoiceRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Billing.Invoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Details = new List<InvoiceDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 10, UnitPrice = 100, TaxRate = 16 }
            }
        };

        await _handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Subtotal.Should().Be(1000);
        captured.TaxAmount.Should().Be(160);
        captured.Total.Should().Be(1160);
        captured.BalanceDue.Should().Be(1160);
        captured.AmountPaid.Should().Be(0);
    }

    [Fact]
    public async Task Should_Generate_Invoice_Number_With_Correct_Format()
    {
        var product = new Product { Id = 1, Name = "Item", CostPrice = 10, SellingPrice = 20 };
        Domain.Entities.Billing.Invoice? captured = null;
        _fixture.ProductRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _fixture.InvoiceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Billing.Invoice>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Billing.Invoice, CancellationToken>((inv, _) => captured = inv)
            .ReturnsAsync((Domain.Entities.Billing.Invoice inv, CancellationToken _) => inv);
        _fixture.InvoiceRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Billing.Invoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Details = new List<InvoiceDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 100 }
            }
        };

        await _handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.InvoiceNumber.Should().StartWith("FACB-");
        captured.InvoiceNumber.Should().Contain(DateTime.UtcNow.ToString("yyyyMMdd"));
    }
}
