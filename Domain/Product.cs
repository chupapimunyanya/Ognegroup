using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }
}
