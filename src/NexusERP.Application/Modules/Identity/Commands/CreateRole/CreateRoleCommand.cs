using MediatR;
using NexusERP.Application.Common.Models;

namespace NexusERP.Application.Modules.Identity.Commands.CreateRole;

public class CreateRoleCommand : IRequest<ApiResponse<int>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
