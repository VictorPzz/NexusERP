using FluentAssertions;
using NexusERP.Application.Modules.Billing.Commands.CreatePayment;
using NexusERP.Domain.Enums;

namespace NexusERP.Tests.Validators;

public class CreatePaymentCommandValidatorTests
{
    private readonly CreatePaymentCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_InvoiceId_Zero()
    {
        var command = new CreatePaymentCommand { InvoiceId = 0, Amount = 100, PaymentMethod = PaymentMethod.cash };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "InvoiceId");
    }

    [Fact]
    public void Should_Fail_When_Amount_Zero()
    {
        var command = new CreatePaymentCommand { InvoiceId = 1, Amount = 0, PaymentMethod = PaymentMethod.cash };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public void Should_Fail_When_Amount_Negative()
    {
        var command = new CreatePaymentCommand { InvoiceId = 1, Amount = -50, PaymentMethod = PaymentMethod.cash };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_With_Valid_Command()
    {
        var command = new CreatePaymentCommand { InvoiceId = 1, Amount = 500, PaymentMethod = PaymentMethod.transfer };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
