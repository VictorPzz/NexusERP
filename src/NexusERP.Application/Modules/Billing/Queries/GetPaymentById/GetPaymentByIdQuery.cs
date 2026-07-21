using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Billing.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Billing.Queries.GetPaymentById;

public class GetPaymentByIdQuery : IRequest<ApiResponse<PaymentDto>>
{
    public int Id { get; set; }
}

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, ApiResponse<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
            return ApiResponse<PaymentDto>.NotFound("Payment not found");

        var invoice = await _unitOfWork.Invoices.GetByIdAsync(payment.InvoiceId, cancellationToken);

        var dto = new PaymentDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            InvoiceId = payment.InvoiceId,
            InvoiceNumber = invoice?.InvoiceNumber,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentDate = payment.PaymentDate,
            Reference = payment.Reference,
            Notes = payment.Notes,
            CreatedBy = payment.CreatedBy
        };

        return ApiResponse<PaymentDto>.Ok(dto);
    }
}
