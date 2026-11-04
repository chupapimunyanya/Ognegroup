using Application.Interfaces;
using Domain;
using MediatR;
using Ognegroup.Application.Exceptions;

namespace Application.Commands;

// DTO for handling items in the shopping cart
public class CartItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderCommand : IRequest<int>
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,
            ShippingAddress = request.ShippingAddress,
            OrderDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        foreach (var cartItem in request.Items)
        {
            // Get the product from the database to know its REAL price at the moment
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new NotFoundException($"Product with ID {cartItem.ProductId} not found.");
            }

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = cartItem.Quantity,
                UnitPrice = product.RetailPrice // FIXING THE PRICE! If it changes tomorrow, this order will not be affected.
            };

            order.Items.Add(orderItem);

            // Calculate the total amount of the order
            order.TotalAmount += orderItem.UnitPrice * orderItem.Quantity;
        }

        // Save the completed order to the database
        return await _orderRepository.CreateOrderAsync(order, cancellationToken);
    }
}