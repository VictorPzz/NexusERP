using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.People.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithPersonAsync(request.Id, cancellationToken);
        if (employee is null)
            return ApiResponse.NotFound("Employee not found");

        if (request.FirstName is not null) employee.Person.FirstName = request.FirstName;
        if (request.LastName is not null) employee.Person.LastName = request.LastName;
        if (request.Email is not null) employee.Person.Email = request.Email;
        if (request.Phone is not null) employee.Person.Phone = request.Phone;
        if (request.Mobile is not null) employee.Person.Mobile = request.Mobile;
        if (request.JobPositionId.HasValue) employee.JobPositionId = request.JobPositionId.Value;
        if (request.Salary.HasValue) employee.Salary = request.Salary;
        if (request.Status is not null && Enum.TryParse<EmployeeStatus>(request.Status, true, out var status))
            employee.Status = status;

        employee.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Employee updated successfully");
    }
}
