using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return Ok(new { OrderId = orderId });
    }

    // GET: api/orders
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var query = new GetOrdersQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}