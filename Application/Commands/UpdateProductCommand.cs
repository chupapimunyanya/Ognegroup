using Application.Interfaces;
using MediatR;
using Ognegroup.Application.Exceptions;


namespace Ognegroup.Application.Commands;

public class UpdateProductCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

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

        // Throw our custom exception if the product doesn't exist
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.Id} not found.");
        }

        product.Name = request.Name;
        product.Price = request.Price;

        await _repository.UpdateAsync(product);
    }
}