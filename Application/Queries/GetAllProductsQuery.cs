using Application.Interfaces;
using MediatR;

namespace Application.Queries;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // NEW
    public string ImageUrl { get; set; } = string.Empty;    // NEW
    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetAllProductsQuery : IRequest<PaginatedResult<ProductDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.Page, request.PageSize, request.SearchTerm);

        var dtos = items.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description, 
            ImageUrl = p.ImageUrl,       
            CostPrice = p.CostPrice,
            RetailPrice = p.RetailPrice,
            ExternalId = p.ExternalId,
            SupplierName = p.Supplier?.Name ?? "Unknown"
        });

        return new PaginatedResult<ProductDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}