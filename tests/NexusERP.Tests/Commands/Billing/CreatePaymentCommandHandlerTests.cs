using FluentAssertions;
using Moq;
using NexusERP.Application.Modules.Billing.Commands.CreatePayment;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Entities.Billing;
using NexusERP.Tests.Fixtures;

namespace NexusERP.Tests.Commands.Billing;

public class CreatePaymentCommandHandlerTests : IDisposable
{
    private readonly MockUnitOfWorkFixture _fixture = new();
    private readonly CreatePaymentCommandHandler _handler;

    public CreatePaymentCommandHandlerTests()
    {
        _handler = new CreatePaymentCommandHandler(
            _fixture.UnitOfWorkMock.Object,
            _fixture.CurrentUserServiceMock.Object);
    }

    public void Dispose() { }

    private Invoice CreateTestInvoice(decimal total = 1000, decimal amountPaid = 0, InvoiceStatus status = InvoiceStatus.issued)
    {
        return new Invoice
        {
            Id = 1,
            InvoiceNumber = "FACB-20260721-0001",
            InvoiceType = InvoiceType.sale,
            ClientId = 1,
            Total = total,
            AmountPaid = amountPaid,
            BalanceDue = total - amountPaid,
            Status = status
        };
    }

    [Fact]
    public async Task Should_Record_Payment_Successfully()
    {
        var invoice = CreateTestInvoice(total: 1000, amountPaid: 0, status: InvoiceStatus.issued);
        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);
        _fixture.PaymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment p, CancellationToken _) => { p.Id = 1; return p; });
        _fixture.PaymentRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreatePaymentCommand
        {
            InvoiceId = 1,
            Amount = 500,
            PaymentMethod = PaymentMethod.cash,
            Reference = "REF-001"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().BeGreaterThan(0);
        invoice.AmountPaid.Should().Be(500);
        invoice.BalanceDue.Should().Be(500);
        invoice.Status.Should().Be(InvoiceStatus.issued);
    }

    [Fact]
    public async Task Should_Auto_Pay_Invoice_When_Full_Amount_Paid()
    {
        var invoice = CreateTestInvoice(total: 1000, amountPaid: 0, status: InvoiceStatus.issued);
        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);
        _fixture.PaymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment p, CancellationToken _) => { p.Id = 1; return p; });
        _fixture.PaymentRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreatePaymentCommand
        {
            InvoiceId = 1,
            Amount = 1000,
            PaymentMethod = PaymentMethod.transfer,
            Reference = "REF-FULL"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        invoice.AmountPaid.Should().Be(1000);
        invoice.BalanceDue.Should().Be(0);
        invoice.Status.Should().Be(InvoiceStatus.paid);
    }

    [Fact]
    public async Task Should_Return_Fail_When_Invoice_Not_Found()
    {
        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var command = new CreatePaymentCommand { InvoiceId = 999, Amount = 100, PaymentMethod = PaymentMethod.cash };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task Should_Return_Fail_When_Invoice_Cancelled()
    {
        var invoice = CreateTestInvoice(status: InvoiceStatus.cancelled);
        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

        var command = new CreatePaymentCommand { InvoiceId = 1, Amount = 100, PaymentMethod = PaymentMethod.cash };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("cancelled");
    }

    [Fact]
    public async Task Should_Return_Fail_When_Invoice_Already_Paid()
    {
        var invoice = CreateTestInvoice(total: 1000, amountPaid: 1000, status: InvoiceStatus.paid);
        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

        var command = new CreatePaymentCommand { InvoiceId = 1, Amount = 100, PaymentMethod = PaymentMethod.cash };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already fully paid");
    }

    [Fact]
    public async Task Should_Return_Fail_When_Amount_Exceeds_Balance()
    {
        var invoice = CreateTestInvoice(total: 1000, amountPaid: 800, status: InvoiceStatus.issued);
        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

        var command = new CreatePaymentCommand { InvoiceId = 1, Amount = 500, PaymentMethod = PaymentMethod.cash };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("exceeds balance");
    }

    [Fact]
    public async Task Should_Generate_Payment_Number_With_Correct_Format()
    {
        var invoice = CreateTestInvoice(total: 500);
        Payment? captured = null;
        _fixture.InvoiceRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);
        _fixture.PaymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Callback<Payment, CancellationToken>((p, _) => captured = p)
            .ReturnsAsync((Payment p, CancellationToken _) => p);
        _fixture.PaymentRepositoryMock.Setup(r => r.CountAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new CreatePaymentCommand { InvoiceId = 1, Amount = 500, PaymentMethod = PaymentMethod.cash };

        await _handler.Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.PaymentNumber.Should().StartWith("PAGO-");
        captured.PaymentNumber.Should().Contain(DateTime.UtcNow.ToString("yyyyMMdd"));
    }
}
