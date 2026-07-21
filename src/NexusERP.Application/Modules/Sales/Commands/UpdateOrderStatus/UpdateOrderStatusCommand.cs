using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Sales.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
}

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
            return ApiResponse.NotFound("Order not found");

        order.Status = request.Status;
        order.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Order status updated successfully");
    }
}
