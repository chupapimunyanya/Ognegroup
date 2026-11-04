namespace Domain;

public class OrderItem
{
    public int Id { get; set; }

    // Link to the Order
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    // Link to the Product
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    // We save the price at the exact moment of purchase.
    // If the product price changes tomorrow, this order remains unchanged.
    public decimal UnitPrice { get; set; }
}