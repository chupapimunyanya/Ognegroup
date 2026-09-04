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
}