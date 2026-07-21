using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Billing.Commands.UpdatePaymentStatus;

public class UpdatePaymentStatusCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdatePaymentStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
            return ApiResponse.NotFound("Payment not found");

        if (request.Reference is not null)
            payment.Reference = request.Reference;
        if (request.Notes is not null)
            payment.Notes = request.Notes;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Payment updated successfully");
    }
}
