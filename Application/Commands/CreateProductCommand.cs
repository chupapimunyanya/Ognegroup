using Application.Interfaces;
using Domain;
using MediatR;

namespace Application.Commands;

// Command definition for creating a new product
public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public bool IsAvailable { get; set; } = true;
}

// Handler that executes the creation logic
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Map command data to the updated domain entity
        var product = new Product
        {
            Name = request.Name,
            CostPrice = request.CostPrice,
            RetailPrice = request.RetailPrice,
            ExternalId = request.ExternalId,
            SupplierId = request.SupplierId,
            IsAvailable = request.IsAvailable
        };

        await _repository.AddAsync(product);

        return product.Id;
    }
}