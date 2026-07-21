using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.People.Queries.GetAllSuppliers;

public class GetAllSuppliersQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<SupplierDto>>>
{
}

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, ApiResponse<PagedResult<SupplierDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllSuppliersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<SupplierDto>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var allSuppliers = await _unitOfWork.Suppliers.GetAllWithPersonAsync(cancellationToken);
        var query = allSuppliers.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(s =>
                s.Person.FirstName.ToLower().Contains(search) ||
                s.Person.LastName.ToLower().Contains(search) ||
                s.SupplierCode.ToLower().Contains(search));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                SupplierCode = s.SupplierCode,
                Person = new PersonDto
                {
                    Id = s.Person.Id,
                    DocumentType = s.Person.DocumentType,
                    DocumentNumber = s.Person.DocumentNumber,
                    FirstName = s.Person.FirstName,
                    MiddleName = s.Person.MiddleName,
                    LastName = s.Person.LastName,
                    SecondLastName = s.Person.SecondLastName,
                    Email = s.Person.Email,
                    Phone = s.Person.Phone,
                    Mobile = s.Person.Mobile,
                    DateOfBirth = s.Person.DateOfBirth,
                    Gender = s.Person.Gender
                },
                CompanyName = s.CompanyName,
                TaxId = s.TaxId,
                Website = s.Website,
                PaymentTerms = s.PaymentTerms,
                Rating = s.Rating,
                Status = s.Status,
                Notes = s.Notes
            })
            .ToList();

        var pagedResult = new PagedResult<SupplierDto>(items, totalCount, request.Page, request.PageSize);
        return ApiResponse<PagedResult<SupplierDto>>.Ok(pagedResult);
    }
}
