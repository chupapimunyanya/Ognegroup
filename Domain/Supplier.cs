namespace Domain; // Убедитесь, что namespace совпадает с вашим

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Глобальная наценка на товары этого поставщика (например, 20%)
    public decimal DefaultMarkupPercentage { get; set; }

    // Связь: у одного поставщика много товаров
    public ICollection<Product> Products { get; set; } = new List<Product>();
}