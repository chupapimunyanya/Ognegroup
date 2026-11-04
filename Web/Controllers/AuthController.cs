using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        // MediatR routes the command to LoginCommandHandler
        var token = await _mediator.Send(command);

        // Return token as a JSON object
        return Ok(new { Token = token });
    }
}