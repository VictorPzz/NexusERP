using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.People.Queries.GetAllClients;

public class GetAllClientsQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<ClientDto>>>
{
}

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, ApiResponse<PagedResult<ClientDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllClientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ClientDto>>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        var allClients = await _unitOfWork.Clients.GetAllWithPersonAsync(cancellationToken);
        var query = allClients.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(c =>
                c.Person.FirstName.ToLower().Contains(search) ||
                c.Person.LastName.ToLower().Contains(search) ||
                c.ClientCode.ToLower().Contains(search));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ClientDto
            {
                Id = c.Id,
                ClientCode = c.ClientCode,
                Person = new PersonDto
                {
                    Id = c.Person.Id,
                    DocumentType = c.Person.DocumentType,
                    DocumentNumber = c.Person.DocumentNumber,
                    FirstName = c.Person.FirstName,
                    MiddleName = c.Person.MiddleName,
                    LastName = c.Person.LastName,
                    SecondLastName = c.Person.SecondLastName,
                    Email = c.Person.Email,
                    Phone = c.Person.Phone,
                    Mobile = c.Person.Mobile,
                    DateOfBirth = c.Person.DateOfBirth,
                    Gender = c.Person.Gender
                },
                CreditLimit = c.CreditLimit,
                CurrentBalance = c.CurrentBalance,
                Status = c.Status,
                Notes = c.Notes,
                AddressCount = c.Addresses.Count
            })
            .ToList();

        var pagedResult = new PagedResult<ClientDto>(items, totalCount, request.Page, request.PageSize);
        return ApiResponse<PagedResult<ClientDto>>.Ok(pagedResult);
    }
}
