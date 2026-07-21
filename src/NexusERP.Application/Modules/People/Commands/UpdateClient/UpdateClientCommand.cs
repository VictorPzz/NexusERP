using MediatR;
using NexusERP.Application.Common.Models;

namespace NexusERP.Application.Modules.People.Commands.UpdateClient;

public class UpdateClientCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal? CreditLimit { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
}

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ApiResponse>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;
    private readonly Domain.Interfaces.Services.ICurrentUserService _currentUser;

    public UpdateClientCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, Domain.Interfaces.Services.ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdWithPersonAsync(request.Id, cancellationToken);
        if (client is null)
            return ApiResponse.NotFound("Client not found");

        if (request.FirstName is not null) client.Person.FirstName = request.FirstName;
        if (request.LastName is not null) client.Person.LastName = request.LastName;
        if (request.Email is not null) client.Person.Email = request.Email;
        if (request.Phone is not null) client.Person.Phone = request.Phone;
        if (request.CreditLimit.HasValue) client.CreditLimit = request.CreditLimit.Value;
        if (request.Notes is not null) client.Notes = request.Notes;
        if (request.Status is not null && Enum.TryParse<Domain.Enums.ClientStatus>(request.Status, true, out var status))
            client.Status = status;

        client.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Client updated successfully");
    }
}
