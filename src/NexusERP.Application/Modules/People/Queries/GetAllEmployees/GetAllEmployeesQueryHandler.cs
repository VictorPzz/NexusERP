using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.People.Queries.GetAllEmployees;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, ApiResponse<PagedResult<EmployeeDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllEmployeesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<EmployeeDto>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var allEmployees = await _unitOfWork.Employees.GetAllWithPersonAsync(cancellationToken);
        var query = allEmployees.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(e =>
                e.Person.FirstName.ToLower().Contains(search) ||
                e.Person.LastName.ToLower().Contains(search) ||
                e.EmployeeCode.ToLower().Contains(search));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                Person = new PersonDto
                {
                    Id = e.Person.Id,
                    DocumentType = e.Person.DocumentType,
                    DocumentNumber = e.Person.DocumentNumber,
                    FirstName = e.Person.FirstName,
                    MiddleName = e.Person.MiddleName,
                    LastName = e.Person.LastName,
                    SecondLastName = e.Person.SecondLastName,
                    Email = e.Person.Email,
                    Phone = e.Person.Phone,
                    Mobile = e.Person.Mobile,
                    DateOfBirth = e.Person.DateOfBirth,
                    Gender = e.Person.Gender
                },
                JobPositionId = e.JobPositionId,
                JobPositionName = e.JobPosition.Name,
                DepartmentName = e.JobPosition.Department.Name,
                HireDate = e.HireDate,
                TerminationDate = e.TerminationDate,
                EmploymentType = e.EmploymentType,
                Status = e.Status,
                Salary = e.Salary
            })
            .ToList();

        var pagedResult = new PagedResult<EmployeeDto>(items, totalCount, request.Page, request.PageSize);
        return ApiResponse<PagedResult<EmployeeDto>>.Ok(pagedResult);
    }
}
