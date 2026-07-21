using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Commands.RemoveRole;

public class RemoveRoleCommand : IRequest<ApiResponse>
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdWithRolesAsync(request.UserId, cancellationToken);
        if (user is null)
            return ApiResponse.NotFound("User not found");

        var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == request.RoleId);
        if (userRole is null)
            return ApiResponse.Fail("User does not have this role");

        user.UserRoles.Remove(userRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Role removed successfully");
    }
}
