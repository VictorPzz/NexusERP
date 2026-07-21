using MediatR;
using NexusERP.Application.Common.Models;

namespace NexusERP.Application.Modules.People.Commands.UpdateEmployee;

public class UpdateEmployeeCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public int? JobPositionId { get; set; }
    public decimal? Salary { get; set; }
    public string? Status { get; set; }
}
