using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductParser _parser;

    public ProductsController(IProductParser parser)
    {
        _parser = parser;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        return Ok(_parser.GetProducts());
    }
}