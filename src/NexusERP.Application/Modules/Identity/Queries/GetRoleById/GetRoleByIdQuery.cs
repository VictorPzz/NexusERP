using MediatR;
using NexusERP.Application.Common.Models;

namespace NexusERP.Application.Modules.Identity.Queries.GetRoleById;

public class GetRoleByIdQuery : IRequest<ApiResponse<RoleDetailDto>>
{
    public int Id { get; set; }
}

public class RoleDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UserInfoDto> Users { get; set; } = new();
}

public class UserInfoDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
