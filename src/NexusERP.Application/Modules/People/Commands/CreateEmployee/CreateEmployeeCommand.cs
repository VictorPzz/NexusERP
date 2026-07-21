using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;
using NexusERP.Domain.Enums;

namespace NexusERP.Application.Modules.People.Commands.CreateEmployee;

public class CreateEmployeeCommand : IRequest<ApiResponse<int>>
{
    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? SecondLastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public int JobPositionId { get; set; }
    public DateTime HireDate { get; set; }
    public EmploymentType EmploymentType { get; set; } = EmploymentType.full_time;
    public decimal? Salary { get; set; }
}
