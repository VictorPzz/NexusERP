using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Sales.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Sales.Queries.GetSaleById;

public class GetSaleByIdQuery : IRequest<ApiResponse<SaleDto>>
{
    public int Id { get; set; }
}

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, ApiResponse<SaleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSaleByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<SaleDto>> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.Sales.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (sale is null)
            return ApiResponse<SaleDto>.NotFound("Sale not found");

        var dto = new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            ClientId = sale.ClientId,
            ClientName = $"{sale.Client.Person.FirstName} {sale.Client.Person.LastName}",
            SaleDate = sale.SaleDate,
            Status = sale.Status,
            PaymentStatus = sale.PaymentStatus,
            Subtotal = sale.Subtotal,
            TaxAmount = sale.TaxAmount,
            DiscountAmount = sale.DiscountAmount,
            Total = sale.Total,
            Notes = sale.Notes,
            ItemCount = sale.Details.Count
        };

        return ApiResponse<SaleDto>.Ok(dto);
    }
}
