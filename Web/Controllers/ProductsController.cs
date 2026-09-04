using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ognegroup.Application.Commands;
using Ognegroup.Application.Queries;

namespace Ognegroup.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateProductCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductCommand command)
    {
        command.Id = id; // Ensure the ID from the URL is passed to the command
        await _mediator.Send(command);
        return NoContent(); // 204 No Content is standard for successful updates
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteProductCommand { Id = id });
        return NoContent();
    }
}