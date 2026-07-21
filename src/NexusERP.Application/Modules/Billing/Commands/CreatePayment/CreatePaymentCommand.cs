using FluentValidation;
using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Billing.Commands.CreatePayment;

public class CreatePaymentCommand : IRequest<ApiResponse<int>>
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invoice is required");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero");
    }
}

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreatePaymentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return ApiResponse<int>.NotFound("Invoice not found");

        if (invoice.Status == InvoiceStatus.cancelled)
            return ApiResponse<int>.Fail("Cannot record payment for a cancelled invoice");

        if (invoice.Status == InvoiceStatus.paid)
            return ApiResponse<int>.Fail("Invoice is already fully paid");

        if (request.Amount > invoice.BalanceDue)
            return ApiResponse<int>.Fail($"Payment amount exceeds balance due of {invoice.BalanceDue:C}");

        var paymentNumber = await GeneratePaymentNumberAsync(cancellationToken);

        var payment = new Domain.Entities.Billing.Payment
        {
            PaymentNumber = paymentNumber,
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaymentDate = DateTime.UtcNow,
            Reference = request.Reference,
            Notes = request.Notes,
            CreatedBy = _currentUser.UserId!.Value
        };

        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);

        invoice.AmountPaid += request.Amount;
        invoice.BalanceDue = invoice.Total - invoice.AmountPaid;
        invoice.Status = invoice.BalanceDue <= 0 ? InvoiceStatus.paid : InvoiceStatus.issued;
        invoice.UpdatedBy = _currentUser.UserId;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(payment.Id, "Payment recorded successfully");
    }

    private async Task<string> GeneratePaymentNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _unitOfWork.Payments.CountAsync(p => p.PaymentNumber.StartsWith($"PAGO-{today}"), cancellationToken);
        return $"PAGO-{today}-{(count + 1):D4}";
    }
}
