using Application.Interfaces;
using MediatR;
using Ognegroup.Application.Exceptions;


namespace Ognegroup.Application.Commands;

public class DeleteProductCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repository;

    public DeleteProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);

        // Throw our custom exception if the product doesn't exist
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.Id} not found.");
        }

        await _repository.DeleteAsync(request.Id);
    }
}