using MediatR;
using NexusERP.Application.Common.Models;

namespace NexusERP.Application.Modules.People.Commands.UpdateSupplier;

public class UpdateSupplierCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? Website { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
}

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, ApiResponse>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;
    private readonly Domain.Interfaces.Services.ICurrentUserService _currentUser;

    public UpdateSupplierCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, Domain.Interfaces.Services.ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdWithPersonAsync(request.Id, cancellationToken);
        if (supplier is null)
            return ApiResponse.NotFound("Supplier not found");

        if (request.FirstName is not null) supplier.Person.FirstName = request.FirstName;
        if (request.LastName is not null) supplier.Person.LastName = request.LastName;
        if (request.Email is not null) supplier.Person.Email = request.Email;
        if (request.Phone is not null) supplier.Person.Phone = request.Phone;
        if (request.CompanyName is not null) supplier.CompanyName = request.CompanyName;
        if (request.TaxId is not null) supplier.TaxId = request.TaxId;
        if (request.Website is not null) supplier.Website = request.Website;
        if (request.PaymentTerms is not null) supplier.PaymentTerms = request.PaymentTerms;
        if (request.Notes is not null) supplier.Notes = request.Notes;
        if (request.Status is not null && Enum.TryParse<Domain.Enums.SupplierStatus>(request.Status, true, out var status))
            supplier.Status = status;

        supplier.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Supplier updated successfully");
    }
}
