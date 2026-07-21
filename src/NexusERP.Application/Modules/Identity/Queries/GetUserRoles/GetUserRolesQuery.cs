using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Identity.Queries.GetUserById;

namespace NexusERP.Application.Modules.Identity.Queries.GetUserRoles;

public class GetUserRolesQuery : IRequest<ApiResponse<List<RoleInfoDto>>>
{
    public int UserId { get; set; }
}
