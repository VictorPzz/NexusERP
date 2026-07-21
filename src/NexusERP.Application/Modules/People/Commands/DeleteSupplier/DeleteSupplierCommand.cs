using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.People.Commands.DeleteSupplier;

public class DeleteSupplierCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
}

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteSupplierCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return ApiResponse.NotFound("Supplier not found");

        supplier.IsDeleted = true;
        supplier.Status = SupplierStatus.inactive;
        supplier.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Supplier deleted successfully");
    }
}
