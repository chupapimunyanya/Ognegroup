using Domain;
using Application.Interfaces;
using MediatR;

namespace Application.Commands;

// Command definition for creating a new supplier
public class CreateSupplierCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public decimal DefaultMarkupPercentage { get; set; }
}

// Handler that executes the creation logic
public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, int>
{
    private readonly ISupplierRepository _repository;

    public CreateSupplierCommandHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier
        {
            Name = request.Name,
            DefaultMarkupPercentage = request.DefaultMarkupPercentage
        };

        await _repository.AddAsync(supplier, cancellationToken);

        return supplier.Id;
    }
}