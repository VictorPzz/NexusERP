using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Sales.Commands.UpdateSaleStatus;

public class UpdateSaleStatusCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public SaleStatus Status { get; set; }
}

public class UpdateSaleStatusCommandHandler : IRequestHandler<UpdateSaleStatusCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateSaleStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdateSaleStatusCommand request, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.Sales.GetByIdAsync(request.Id, cancellationToken);
        if (sale is null)
            return ApiResponse.NotFound("Sale not found");

        sale.Status = request.Status;
        sale.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Sale status updated successfully");
    }
}
