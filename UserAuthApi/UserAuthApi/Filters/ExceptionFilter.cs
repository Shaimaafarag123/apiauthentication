using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserAuthApi.Models;

namespace UserAuthApi.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

           
            var logger = context.HttpContext.RequestServices.GetService<ILogger<ExceptionFilter>>();
            logger?.LogError(exception, "Unhandled exception in controller");

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = 500,
                Title = "An error occurred.",
                Detail = exception.Message
            })
            {
                StatusCode = 500
            };
        }
    }
}
