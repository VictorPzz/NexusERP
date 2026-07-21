using MediatR;
using NexusERP.Application.Common.Models;

namespace NexusERP.Application.Modules.Identity.Commands.CreateUser;

public class CreateUserCommand : IRequest<ApiResponse<int>>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? RoleId { get; set; }
}
