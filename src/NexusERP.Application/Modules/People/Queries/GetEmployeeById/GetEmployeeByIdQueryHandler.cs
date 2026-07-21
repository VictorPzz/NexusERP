using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.People.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, ApiResponse<EmployeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEmployeeByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithPersonAsync(request.Id, cancellationToken);
        if (employee is null)
            return ApiResponse<EmployeeDto>.NotFound("Employee not found");

        var dto = new EmployeeDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            Person = new PersonDto
            {
                Id = employee.Person.Id,
                DocumentType = employee.Person.DocumentType,
                DocumentNumber = employee.Person.DocumentNumber,
                FirstName = employee.Person.FirstName,
                MiddleName = employee.Person.MiddleName,
                LastName = employee.Person.LastName,
                SecondLastName = employee.Person.SecondLastName,
                Email = employee.Person.Email,
                Phone = employee.Person.Phone,
                Mobile = employee.Person.Mobile,
                DateOfBirth = employee.Person.DateOfBirth,
                Gender = employee.Person.Gender
            },
            JobPositionId = employee.JobPositionId,
            JobPositionName = employee.JobPosition.Name,
            DepartmentName = employee.JobPosition.Department.Name,
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            EmploymentType = employee.EmploymentType,
            Status = employee.Status,
            Salary = employee.Salary
        };

        return ApiResponse<EmployeeDto>.Ok(dto);
    }
}
