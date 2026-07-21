using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Commands.DeleteRole;

public class DeleteRoleCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
}

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return ApiResponse.NotFound("Role not found");

        role.IsActive = false;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Role deactivated successfully");
    }
}
