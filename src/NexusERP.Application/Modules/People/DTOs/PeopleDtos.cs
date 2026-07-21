using NexusERP.Domain.Enums;

namespace NexusERP.Application.Modules.People.DTOs;

public class PersonDto
{
    public int Id { get; set; }
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
}

public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public PersonDto Person { get; set; } = null!;
    public int JobPositionId { get; set; }
    public string JobPositionName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public EmployeeStatus Status { get; set; }
    public decimal? Salary { get; set; }
}

public class ClientDto
{
    public int Id { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public PersonDto Person { get; set; } = null!;
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public ClientStatus Status { get; set; }
    public string? Notes { get; set; }
    public int AddressCount { get; set; }
}

public class SupplierDto
{
    public int Id { get; set; }
    public string SupplierCode { get; set; } = string.Empty;
    public PersonDto Person { get; set; } = null!;
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? Website { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal? Rating { get; set; }
    public SupplierStatus Status { get; set; }
    public string? Notes { get; set; }
}
