using Application.Interfaces;
using MediatR;

namespace Application.Queries;

// DTO for the order item (only the necessary information)
public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

// DTO for the order itself
public class OrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class GetOrdersQuery : IRequest<IEnumerable<OrderDto>>
{
}

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetAllOrdersAsync(cancellationToken);

        // Mapping complicated entities to clean DTOs
        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerName = o.CustomerName,
            CustomerEmail = o.CustomerEmail,
            CustomerPhone = o.CustomerPhone,
            ShippingAddress = o.ShippingAddress,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Items = o.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Unknown Product",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        });
    }
}