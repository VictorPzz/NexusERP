using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.People.Commands.DeleteClient;

public class DeleteClientCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
}

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteClientCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.Id, cancellationToken);
        if (client is null)
            return ApiResponse.NotFound("Client not found");

        client.IsDeleted = true;
        client.Status = ClientStatus.inactive;
        client.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Client deleted successfully");
    }
}
