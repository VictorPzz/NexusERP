using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Identity.Queries.GetUserById;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Queries.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, ApiResponse<List<RoleInfoDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserRolesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<RoleInfoDto>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdWithRolesAsync(request.UserId, cancellationToken);
        if (user is null)
            return ApiResponse<List<RoleInfoDto>>.NotFound("User not found");

        var roles = user.UserRoles.Select(ur => new RoleInfoDto
        {
            Id = ur.Role.Id,
            Name = ur.Role.Name,
            Description = ur.Role.Description
        }).ToList();

        return ApiResponse<List<RoleInfoDto>>.Ok(roles);
    }
}
