using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Presentation;

public static class ErrorHandler
{
    public static ActionResult Handle<T>(ErrorOr<T> result)
    {
        var error = result.Errors.First();
        return error.Type switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(error.Description),
            ErrorType.Validation => new BadRequestObjectResult(error.Description),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(error.Description),
            ErrorType.Forbidden => new ForbidResult(),
            _ => new StatusCodeResult(500)
        };
    }
}