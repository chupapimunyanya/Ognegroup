using Application.Interfaces;
using Domain;
using Infrastructure.Data; // Removed 'Ognegroup.' prefix
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories; // Removed 'Ognegroup.' prefix

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Supplier) // Eagerly load the related Supplier data
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        // Changed from FindAsync to FirstOrDefaultAsync to support .Include()
        return await _context.Products
            .Include(p => p.Supplier) // Eagerly load the related Supplier data
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync(); // Save changes to the database
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    // Add this method inside your ProductRepository class
    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm)
    {
        var query = _context.Products
            .Include(p => p.Supplier)
            .AsQueryable();

        // 1. Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        // 2. Count total items BEFORE pagination (needed for the frontend to draw page numbers)
        var totalCount = await query.CountAsync();

        // 3. Apply pagination (Skip previous pages, Take current page size)
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}