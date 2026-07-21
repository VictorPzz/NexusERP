using MediatR;
using NexusERP.Application.Common.Models;

namespace NexusERP.Application.Modules.Identity.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
}
