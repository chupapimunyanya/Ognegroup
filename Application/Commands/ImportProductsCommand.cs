using Ognegroup.Application.Exceptions;
using Application.Interfaces;
using Domain;
using MediatR;

namespace Application.Commands;

// Added Description and ImageUrl to the import DTO
public class ProductImportDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public string ExternalId { get; set; } = string.Empty;
}

public class ImportProductsCommand : IRequest<int>
{
    public int SupplierId { get; set; }
    public List<ProductImportDto> Items { get; set; } = new();
}

public class ImportProductsCommandHandler : IRequestHandler<ImportProductsCommand, int>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public ImportProductsCommandHandler(IProductRepository productRepository, ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<int> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);
        if (supplier == null)
        {
            throw new NotFoundException($"Supplier with ID {request.SupplierId} not found.");
        }

        var allProducts = await _productRepository.GetAllAsync();
        int processedCount = 0;

        foreach (var item in request.Items)
        {
            var markupAmount = item.CostPrice * (supplier.DefaultMarkupPercentage / 100);
            var retailPrice = item.CostPrice + markupAmount;

            var existingProduct = allProducts.FirstOrDefault(p => p.ExternalId == item.ExternalId && p.SupplierId == supplier.Id);

            if (existingProduct != null)
            {
                // Update existing product with new details
                existingProduct.CostPrice = item.CostPrice;
                existingProduct.RetailPrice = retailPrice;
                existingProduct.Name = item.Name;
                existingProduct.Description = item.Description; // NEW
                existingProduct.ImageUrl = item.ImageUrl;       // NEW

                await _productRepository.UpdateAsync(existingProduct);
            }
            else
            {
                // Create new product with all details
                var newProduct = new Product
                {
                    Name = item.Name,
                    Description = item.Description, // NEW
                    ImageUrl = item.ImageUrl,       // NEW
                    CostPrice = item.CostPrice,
                    RetailPrice = retailPrice,
                    ExternalId = item.ExternalId,
                    SupplierId = supplier.Id
                };

                await _productRepository.AddAsync(newProduct);
            }

            processedCount++;
        }

        return processedCount;
    }
}