namespace Application.Interfaces;

public interface IJwtProvider
{
    // Generates a JWT token for the authenticated user
    string GenerateToken(string username);
}