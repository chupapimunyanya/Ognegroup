using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id); // Fetch a single product
        Task AddAsync(Product product);      // Add a new product
        Task UpdateAsync(Product product);   // Update existing product
        Task DeleteAsync(int id);            // Delete by ID
    }
}
