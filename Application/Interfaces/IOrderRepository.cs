using Domain;

namespace Application.Interfaces;

public interface IOrderRepository
{
    Task<int> CreateOrderAsync(Order order, CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
}