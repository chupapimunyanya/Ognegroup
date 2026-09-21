using Application.Interfaces;
using MediatR;
using Ognegroup.Application.Exceptions;

namespace Application.Queries;

// Request to get a single product by its ID
public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}

// Handler that fetches the product and maps it to a DTO
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repository;

    public GetProductByIdQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch the product from the database
        var product = await _repository.GetByIdAsync(request.Id);

        // 2. Throw our custom NotFoundException if it doesn't exist
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.Id} not found.");
        }

        // 3. Map to DTO
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            CostPrice = product.CostPrice,
            RetailPrice = product.RetailPrice,
            ExternalId = product.ExternalId,
            SupplierName = product.Supplier?.Name ?? "Unknown",
            IsAvailable = product.IsAvailable
        };
    }
}