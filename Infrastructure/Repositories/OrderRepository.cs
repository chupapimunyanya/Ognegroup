using Application.Interfaces;
using Domain;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return order.Id;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)          // Get order items
                .ThenInclude(i => i.Product) // For every item, include the product
            .OrderByDescending(o => o.OrderDate) // Newest orders first
            .ToListAsync(cancellationToken);
    }
}