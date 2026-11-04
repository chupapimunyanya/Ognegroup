using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Commands;

public class LoginCommand : IRequest<string>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IJwtProvider jwtProvider, IConfiguration configuration)
    {
        _jwtProvider = jwtProvider;
        _configuration = configuration;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Get expected credentials from environment variables (.env)
        var expectedUsername = _configuration["ADMIN_USERNAME"];
        var expectedPassword = _configuration["ADMIN_PASSWORD"];

        if (request.Username != expectedUsername || request.Password != expectedPassword)
        {
            // Throw unauthorized exception
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // Generate and return JWT token
        var token = _jwtProvider.GenerateToken(request.Username);

        return await Task.FromResult(token);
    }
}