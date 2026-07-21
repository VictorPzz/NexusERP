using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using FluentValidation;

namespace NexusERP.Application.Modules.People.Commands.CreateClient;

public class CreateClientCommand : IRequest<ApiResponse<int>>
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
    public string ClientCode { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
    public string? Notes { get; set; }
}

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ClientCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ApiResponse<int>>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;
    private readonly Domain.Interfaces.Services.ICurrentUserService _currentUser;

    public CreateClientCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, Domain.Interfaces.Services.ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Clients.CodeExistsAsync(request.ClientCode, cancellationToken))
            return ApiResponse<int>.Fail("Client code already exists");

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

        var client = new Domain.Entities.People.Client
        {
            PersonId = person.Id,
            ClientCode = request.ClientCode,
            CreditLimit = request.CreditLimit,
            Notes = request.Notes,
            Status = ClientStatus.active,
            CreatedBy = _currentUser.UserId
        };

        await _unitOfWork.Clients.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(client.Id, "Client created successfully");
    }
}
