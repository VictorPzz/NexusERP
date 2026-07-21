using MediatR;
using NexusERP.Application.Common.Models;

namespace NexusERP.Application.Modules.Identity.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<ApiResponse<UserDetailDto>>
{
    public int Id { get; set; }
}

public class UserDetailDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<RoleInfoDto> Roles { get; set; } = new();
}

public class RoleInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
