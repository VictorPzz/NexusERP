using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<RoleDetailDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return ApiResponse<RoleDetailDto>.NotFound("Role not found");

        var dto = new RoleDetailDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            Users = role.UserRoles.Select(ur => new UserInfoDto
            {
                Id = ur.User.Id,
                Username = ur.User.Username,
                Email = ur.User.Email
            }).ToList()
        };

        return ApiResponse<RoleDetailDto>.Ok(dto);
    }
}
