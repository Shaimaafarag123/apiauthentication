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

            var statuCode = context.HttpContext.Response.StatusCode;
           
            var logger = context.HttpContext.RequestServices.GetService<ILogger<ExceptionFilter>>();
            logger?.LogError(exception, "Unhandled exception in controller");


            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = statuCode,
                Title = exception.Message,
                Detail = exception.Message
            })
            {
                StatusCode = statuCode,
                Value = exception.Message
            };
        }
    }
}
