using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;

namespace NexusERP.Application.Modules.People.Queries.GetAllEmployees;

public class GetAllEmployeesQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<EmployeeDto>>>
{
}
