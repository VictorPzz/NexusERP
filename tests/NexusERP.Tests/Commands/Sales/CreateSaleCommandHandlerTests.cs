using FluentAssertions;
using Moq;
using NexusERP.Application.Modules.Sales.Commands.CreateSale;
using NexusERP.Application.Modules.Sales.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Entities.People;
using NexusERP.Tests.Fixtures;

namespace NexusERP.Tests.Commands.Sales;

public class CreateSaleCommandHandlerTests : IDisposable
{
    private readonly MockUnitOfWorkFixture _fixture = new();
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerTests()
    {
        _handler = new CreateSaleCommandHandler(
            _fixture.UnitOfWorkMock.Object,
            _fixture.CurrentUserServiceMock.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task Should_Create_Sale_Successfully()
    {
        var product = new Product { Id = 1, Name = "Laptop", CostPrice = 800, SellingPrice = 1200 };
        var client = new Client { Id = 1, PersonId = 1, ClientCode = "CLI001" };
        _fixture.ProductRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _fixture.ClientRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        _fixture.SaleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Sales.Sale>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Sales.Sale s, CancellationToken _) => { s.Id = 1; return s; });
        _fixture.SaleRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Sales.Sale, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreateSaleCommand
        {
            ClientId = 1,
            DiscountAmount = 0,
            Details = new List<SaleDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 1200, TaxRate = 16, DiscountRate = 0 }
            }
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Return_Fail_When_Client_Not_Found()
    {
        _fixture.ClientRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        var command = new CreateSaleCommand
        {
            ClientId = 999,
            Details = new List<SaleDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 100, TaxRate = 16, DiscountRate = 0 }
            }
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Client not found");
    }

    [Fact]
    public async Task Should_Calculate_Sale_Totals_Correctly()
    {
        var product = new Product { Id = 1, Name = "Widget", CostPrice = 10, SellingPrice = 20 };
        var client = new Client { Id = 1, PersonId = 1, ClientCode = "CLI001" };
        Domain.Entities.Sales.Sale? captured = null;
        _fixture.ProductRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _fixture.ClientRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        _fixture.SaleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Sales.Sale>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Sales.Sale, CancellationToken>((s, _) => captured = s)
            .ReturnsAsync((Domain.Entities.Sales.Sale s, CancellationToken _) => s);
        _fixture.SaleRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Sales.Sale, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreateSaleCommand
        {
            ClientId = 1,
            DiscountAmount = 100,
            Details = new List<SaleDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 10, UnitPrice = 100, TaxRate = 16, DiscountRate = 0 }
            }
        };

        await _handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Subtotal.Should().Be(1000);
        captured.TaxAmount.Should().Be(160);
        captured.Total.Should().Be(1060);
    }

    [Fact]
    public async Task Should_Generate_Sale_Number_With_Correct_Format()
    {
        var product = new Product { Id = 1, Name = "Item", CostPrice = 5, SellingPrice = 10 };
        var client = new Client { Id = 1, PersonId = 1, ClientCode = "CLI001" };
        Domain.Entities.Sales.Sale? captured = null;
        _fixture.ProductRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _fixture.ClientRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        _fixture.SaleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Sales.Sale>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Sales.Sale, CancellationToken>((s, _) => captured = s)
            .ReturnsAsync((Domain.Entities.Sales.Sale s, CancellationToken _) => s);
        _fixture.SaleRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Sales.Sale, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreateSaleCommand
        {
            ClientId = 1,
            Details = new List<SaleDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 10, TaxRate = 0, DiscountRate = 0 }
            }
        };

        await _handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.SaleNumber.Should().StartWith("FAC-");
        captured.SaleNumber.Should().Contain(DateTime.UtcNow.ToString("yyyyMMdd"));
    }
}
