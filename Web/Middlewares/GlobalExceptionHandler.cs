using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ognegroup.Application.Exceptions;

namespace Ognegroup.Web.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Default values for a generic server error
        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "Server Error";
        var detail = "An unexpected error occurred.";

        // If it's our custom NotFoundException, change the response to 404
        if (exception is NotFoundException notFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
            title = "Resource Not Found";
            detail = notFoundException.Message;
        }

        // Build a standardized response model
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Indicates that the exception has been handled
    }
}