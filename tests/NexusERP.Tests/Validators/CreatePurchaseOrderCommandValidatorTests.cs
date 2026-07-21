using FluentAssertions;
using NexusERP.Application.Modules.Purchases.Commands.CreatePurchaseOrder;

namespace NexusERP.Tests.Validators;

public class CreatePurchaseOrderCommandValidatorTests
{
    private readonly CreatePurchaseOrderCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_SupplierId_Zero()
    {
        var command = new CreatePurchaseOrderCommand
        {
            SupplierId = 0,
            Details = new List<Application.Modules.Purchases.DTOs.PurchaseOrderDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 100, TaxRate = 16 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SupplierId");
    }

    [Fact]
    public void Should_Fail_When_Details_Empty()
    {
        var command = new CreatePurchaseOrderCommand { SupplierId = 1 };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_With_Valid_Command()
    {
        var command = new CreatePurchaseOrderCommand
        {
            SupplierId = 1,
            Details = new List<Application.Modules.Purchases.DTOs.PurchaseOrderDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 10, UnitPrice = 50, TaxRate = 16 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
