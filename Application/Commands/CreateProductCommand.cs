using Application.Interfaces;
using Domain;
using MediatR;

namespace Ognegroup.Application.Commands;

// Command. Has only needed data.
public class CreateProductCommand : IRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Handler. Transforms the command into a full-fledged product and saves it.
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };

        await _repository.AddAsync(product);
    }
}