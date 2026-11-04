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

        var allProducts = (await _productRepository.GetAllAsync()).ToList();
        var incomingExternalIds = request.Items.Select(i => i.ExternalId).ToHashSet();
        int processedCount = 0;

        // SYNCHRONIZATION OF AVAILABILITY: Looking for products of this supplier that are NOT in the new price list
        var supplierProductsInDb = allProducts.Where(p => p.SupplierId == supplier.Id).ToList();
        foreach (var existingProduct in supplierProductsInDb)
        {
            if (!incomingExternalIds.Contains(existingProduct.ExternalId))
            {
                existingProduct.IsAvailable = false; // Product is no longer available from the supplier!
                await _productRepository.UpdateAsync(existingProduct);
            }
        }

        // UPDATE AND ADD PRODUCTS
        foreach (var item in request.Items)
        {
            var markupAmount = item.CostPrice * (supplier.DefaultMarkupPercentage / 100);
            var retailPrice = item.CostPrice + markupAmount;

            var existingProduct = supplierProductsInDb.FirstOrDefault(p => p.ExternalId == item.ExternalId);

            if (existingProduct != null)
            {
                existingProduct.CostPrice = item.CostPrice;
                existingProduct.RetailPrice = retailPrice;
                existingProduct.Name = item.Name;
                existingProduct.Description = item.Description;
                existingProduct.ImageUrl = item.ImageUrl;
                existingProduct.IsAvailable = true; // Item is again in the price list, so it's available!

                await _productRepository.UpdateAsync(existingProduct);
            }
            else
            {
                var newProduct = new Product
                {
                    Name = item.Name,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl,
                    CostPrice = item.CostPrice,
                    RetailPrice = retailPrice,
                    ExternalId = item.ExternalId,
                    SupplierId = supplier.Id,
                    IsAvailable = true
                };

                await _productRepository.AddAsync(newProduct);
            }

            processedCount++;
        }

        return processedCount;
    }
}