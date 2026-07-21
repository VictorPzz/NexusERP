using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Entities.People;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.People.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateEmployeeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Employees.CodeExistsAsync(request.EmployeeCode, cancellationToken))
            return ApiResponse<int>.Fail("Employee code already exists");

        var person = new Person
        {
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            SecondLastName = request.SecondLastName,
            Email = request.Email,
            Phone = request.Phone,
            Mobile = request.Mobile,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender
        };

        await _unitOfWork.Persons.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var employee = new Employee
        {
            PersonId = person.Id,
            EmployeeCode = request.EmployeeCode,
            JobPositionId = request.JobPositionId,
            HireDate = request.HireDate,
            EmploymentType = request.EmploymentType,
            Salary = request.Salary,
            Status = Domain.Enums.EmployeeStatus.active,
            CreatedBy = _currentUser.UserId
        };

        await _unitOfWork.Employees.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(employee.Id, "Employee created successfully");
    }
}
