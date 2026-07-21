using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.People.Queries.GetSupplierById;

public class GetSupplierByIdQuery : IRequest<ApiResponse<SupplierDto>>
{
    public int Id { get; set; }
}

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, ApiResponse<SupplierDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSupplierByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdWithPersonAsync(request.Id, cancellationToken);
        if (supplier is null)
            return ApiResponse<SupplierDto>.NotFound("Supplier not found");

        var dto = new SupplierDto
        {
            Id = supplier.Id,
            SupplierCode = supplier.SupplierCode,
            Person = new PersonDto
            {
                Id = supplier.Person.Id,
                DocumentType = supplier.Person.DocumentType,
                DocumentNumber = supplier.Person.DocumentNumber,
                FirstName = supplier.Person.FirstName,
                MiddleName = supplier.Person.MiddleName,
                LastName = supplier.Person.LastName,
                SecondLastName = supplier.Person.SecondLastName,
                Email = supplier.Person.Email,
                Phone = supplier.Person.Phone,
                Mobile = supplier.Person.Mobile,
                DateOfBirth = supplier.Person.DateOfBirth,
                Gender = supplier.Person.Gender
            },
            CompanyName = supplier.CompanyName,
            TaxId = supplier.TaxId,
            Website = supplier.Website,
            PaymentTerms = supplier.PaymentTerms,
            Rating = supplier.Rating,
            Status = supplier.Status,
            Notes = supplier.Notes
        };

        return ApiResponse<SupplierDto>.Ok(dto);
    }
}
