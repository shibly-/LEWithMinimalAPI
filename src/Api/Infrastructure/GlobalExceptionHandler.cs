using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Api.Infrastructure;

/// <summary>
/// Translates application-layer exceptions into RFC 7807 ProblemDetails responses,
/// keeping HTTP concerns at the API boundary and out of the handlers.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {
            case ValidationException validationException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "One or more validation errors occurred.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
                problemDetails.Extensions["errors"] = validationException.Errors;
                break;

            case NotFoundException notFoundException:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "The requested resource was not found.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
                problemDetails.Detail = notFoundException.Message;
                break;

            case BadHttpRequestException badRequestException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "The request could not be processed.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
                problemDetails.Detail = badRequestException.Message;
                break;

            default:
                _logger.LogError(exception, "Unhandled exception processing {Path}", httpContext.Request.Path);
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "An unexpected error occurred.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
