using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.People.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.People.Queries.GetClientById;

public class GetClientByIdQuery : IRequest<ApiResponse<ClientDto>>
{
    public int Id { get; set; }
}

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ApiResponse<ClientDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClientByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ClientDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdWithPersonAsync(request.Id, cancellationToken);
        if (client is null)
            return ApiResponse<ClientDto>.NotFound("Client not found");

        var dto = new ClientDto
        {
            Id = client.Id,
            ClientCode = client.ClientCode,
            Person = new PersonDto
            {
                Id = client.Person.Id,
                DocumentType = client.Person.DocumentType,
                DocumentNumber = client.Person.DocumentNumber,
                FirstName = client.Person.FirstName,
                MiddleName = client.Person.MiddleName,
                LastName = client.Person.LastName,
                SecondLastName = client.Person.SecondLastName,
                Email = client.Person.Email,
                Phone = client.Person.Phone,
                Mobile = client.Person.Mobile,
                DateOfBirth = client.Person.DateOfBirth,
                Gender = client.Person.Gender
            },
            CreditLimit = client.CreditLimit,
            CurrentBalance = client.CurrentBalance,
            Status = client.Status,
            Notes = client.Notes,
            AddressCount = client.Addresses.Count
        };

        return ApiResponse<ClientDto>.Ok(dto);
    }
}
