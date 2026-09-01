using Application;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class MockProductParser : IProductParser
    {
        public IEnumerable<Product> GetProducts()
        {
            return new List<Product>
        {
            new Product { Id = 1, Name = "Беспроводные наушники", Price = 2500, SupplierName = "TechFactory" },
            new Product { Id = 2, Name = "Умные часы", Price = 4200, SupplierName = "TechFactory" },
            new Product { Id = 3, Name = "Городской рюкзак", Price = 1800, SupplierName = "BagHouse" }
        };
        }
    }
}
