namespace Ognegroup.Application.Exceptions;

// Custom exception for missing resources
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}