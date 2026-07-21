using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using FluentValidation;

namespace NexusERP.Application.Modules.People.Commands.CreateSupplier;

public class CreateSupplierCommand : IRequest<ApiResponse<int>>
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
    public string SupplierCode { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? Website { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
}

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SupplierCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, ApiResponse<int>>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;
    private readonly Domain.Interfaces.Services.ICurrentUserService _currentUser;

    public CreateSupplierCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, Domain.Interfaces.Services.ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Suppliers.CodeExistsAsync(request.SupplierCode, cancellationToken))
            return ApiResponse<int>.Fail("Supplier code already exists");

        var person = new Domain.Entities.People.Person
        {
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            SecondLastName = request.SecondLastName,
            Email = request.Email,
            Phone = request.Phone,
            Mobile = request.Mobile
        };

        await _unitOfWork.Persons.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var supplier = new Domain.Entities.People.Supplier
        {
            PersonId = person.Id,
            SupplierCode = request.SupplierCode,
            CompanyName = request.CompanyName,
            TaxId = request.TaxId,
            Website = request.Website,
            PaymentTerms = request.PaymentTerms,
            Notes = request.Notes,
            Status = SupplierStatus.active,
            CreatedBy = _currentUser.UserId
        };

        await _unitOfWork.Suppliers.AddAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(supplier.Id, "Supplier created successfully");
    }
}
