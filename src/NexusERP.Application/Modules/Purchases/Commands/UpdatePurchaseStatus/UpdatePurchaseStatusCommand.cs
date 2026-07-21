using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Purchases.Commands.UpdatePurchaseStatus;

public class UpdatePurchaseStatusCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public PurchaseStatus Status { get; set; }
}

public class UpdatePurchaseStatusCommandHandler : IRequestHandler<UpdatePurchaseStatusCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdatePurchaseStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdatePurchaseStatusCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _unitOfWork.Purchases.GetByIdAsync(request.Id, cancellationToken);
        if (purchase is null)
            return ApiResponse.NotFound("Purchase not found");

        purchase.Status = request.Status;
        purchase.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Purchase status updated successfully");
    }
}
