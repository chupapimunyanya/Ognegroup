using Application.Interfaces;
using MediatR;
using Ognegroup.Application.Exceptions;

namespace Application.Commands;

// Command definition for updating an existing product
public class UpdateProductCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public bool IsAvailable { get; set; } = true;
}

// Handler that executes the update logic
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _repository;

    public UpdateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);

        // Throw global exception if product is missing
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.Id} not found.");
        }

        // Update the entity properties
        product.Name = request.Name;
        product.CostPrice = request.CostPrice;
        product.RetailPrice = request.RetailPrice;
        product.ExternalId = request.ExternalId;
        product.SupplierId = request.SupplierId;
        product.IsAvailable = request.IsAvailable;

        await _repository.UpdateAsync(product);
    }
}