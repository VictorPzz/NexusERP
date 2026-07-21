using FluentAssertions;
using NexusERP.Application.Modules.Billing.Commands.CreateInvoice;
using NexusERP.Domain.Enums;

namespace NexusERP.Tests.Validators;

public class CreateInvoiceCommandValidatorTests
{
    private readonly CreateInvoiceCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Details_Empty()
    {
        var command = new CreateInvoiceCommand { InvoiceType = InvoiceType.sale, ClientId = 1 };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Details");
    }

    [Fact]
    public void Should_Fail_When_Sale_Invoice_Without_Client()
    {
        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.sale,
            Details = new List<Application.Modules.Billing.DTOs.InvoiceDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 100 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ClientId");
    }

    [Fact]
    public void Should_Fail_When_Purchase_Invoice_Without_Supplier()
    {
        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.purchase,
            Details = new List<Application.Modules.Billing.DTOs.InvoiceDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 1, UnitPrice = 100 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SupplierId");
    }

    [Fact]
    public void Should_Pass_With_Valid_Sale_Invoice()
    {
        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Details = new List<Application.Modules.Billing.DTOs.InvoiceDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 100, TaxRate = 16 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_With_Valid_Purchase_Invoice()
    {
        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.purchase,
            SupplierId = 1,
            Details = new List<Application.Modules.Billing.DTOs.InvoiceDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 5, UnitPrice = 50 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Detail_ProductId_Zero()
    {
        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Details = new List<Application.Modules.Billing.DTOs.InvoiceDetailCreateDto>
            {
                new() { ProductId = 0, Quantity = 1, UnitPrice = 100 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Fail_When_Detail_Quantity_Zero()
    {
        var command = new CreateInvoiceCommand
        {
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Details = new List<Application.Modules.Billing.DTOs.InvoiceDetailCreateDto>
            {
                new() { ProductId = 1, Quantity = 0, UnitPrice = 100 }
            }
        };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }
}
