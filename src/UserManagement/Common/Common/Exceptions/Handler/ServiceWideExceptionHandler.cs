using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Common.Exceptions.Handler;

public class ServiceWideExceptionHandler(ILogger<ServiceWideExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(
            "Error Message: {exceptionMessage}, Time of occurrence {time}",
            exception.Message, DateTime.UtcNow);

        var problemDetails = new ProblemDetails
        {
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        problemDetails.Status = exception switch
        {
            DuplicateNameException => StatusCodes.Status409Conflict, // Conflict for duplicate entries.
            KeyNotFoundException => StatusCodes.Status404NotFound, // Not Found for missing keys.
            ValidationException => StatusCodes.Status400BadRequest, // Bad Request for validation errors.
            _ => StatusCodes.Status500InternalServerError // Internal Server Error for all other exceptions.
        };
        context.Response.StatusCode = problemDetails.Status.Value;

        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}
