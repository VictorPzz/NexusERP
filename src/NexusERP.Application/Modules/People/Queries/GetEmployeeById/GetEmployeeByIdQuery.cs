using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;

namespace NexusERP.Application.Modules.People.Queries.GetEmployeeById;

public class GetEmployeeByIdQuery : IRequest<ApiResponse<EmployeeDto>>
{
    public int Id { get; set; }
}
