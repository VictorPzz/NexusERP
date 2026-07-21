using FluentAssertions;
using NexusERP.Application.Modules.Sales.Commands.CreateSale;

namespace NexusERP.Tests.Validators;

public class CreateSaleCommandValidatorTests
{
    private readonly CreateSaleCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_ClientId_Zero()
    {
        var command = new CreateSaleCommand
        {
            ClientId = 0,
            Details = new List<Application.Modules.Sales.DTOs.SaleDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 100, TaxRate = 16, DiscountRate = 0 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ClientId");
    }

    [Fact]
    public void Should_Fail_When_Details_Empty()
    {
        var command = new CreateSaleCommand { ClientId = 1 };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Details");
    }

    [Fact]
    public void Should_Fail_When_DiscountAmount_Negative()
    {
        var command = new CreateSaleCommand
        {
            ClientId = 1,
            DiscountAmount = -10,
            Details = new List<Application.Modules.Sales.DTOs.SaleDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 100, TaxRate = 16, DiscountRate = 0 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_With_Valid_Command()
    {
        var command = new CreateSaleCommand
        {
            ClientId = 1,
            DiscountAmount = 0,
            Details = new List<Application.Modules.Sales.DTOs.SaleDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 100, TaxRate = 16, DiscountRate = 5 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
