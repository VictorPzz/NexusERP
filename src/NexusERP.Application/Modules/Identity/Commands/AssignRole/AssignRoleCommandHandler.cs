using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Entities.Security;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Commands.AssignRole;

public class AssignRoleCommand : IRequest<ApiResponse>
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return ApiResponse.NotFound("User not found");

        var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return ApiResponse.NotFound("Role not found");

        var existingAssignment = user.UserRoles.Any(ur => ur.RoleId == request.RoleId);
        if (existingAssignment)
            return ApiResponse.Fail("User already has this role");

        user.UserRoles.Add(new UserRole
        {
            UserId = request.UserId,
            RoleId = request.RoleId,
            AssignedAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse.Ok("Role assigned successfully");
    }
}
