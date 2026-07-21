using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.People.Commands.DeleteEmployee;

public class DeleteEmployeeCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
}

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteEmployeeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null)
            return ApiResponse.NotFound("Employee not found");

        employee.IsDeleted = true;
        employee.Status = EmployeeStatus.terminated;
        employee.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Employee deleted successfully");
    }
}
