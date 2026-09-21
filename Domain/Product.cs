namespace Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }
    public decimal RetailPrice { get; set; }
    public string ExternalId { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
}