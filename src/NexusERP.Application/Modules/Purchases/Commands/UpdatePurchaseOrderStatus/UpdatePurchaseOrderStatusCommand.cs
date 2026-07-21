using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Purchases.Commands.UpdatePurchaseOrderStatus;

public class UpdatePurchaseOrderStatusCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public PurchaseOrderStatus Status { get; set; }
}

public class UpdatePurchaseOrderStatusCommandHandler : IRequestHandler<UpdatePurchaseOrderStatusCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdatePurchaseOrderStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdatePurchaseOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
            return ApiResponse.NotFound("Purchase order not found");

        order.Status = request.Status;
        order.UpdatedBy = _currentUser.UserId;

        if (request.Status == PurchaseOrderStatus.approved)
        {
            order.ApprovedBy = _currentUser.UserId;
            order.ApprovedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Purchase order status updated successfully");
    }
}
