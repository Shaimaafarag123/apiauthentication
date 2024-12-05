using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using UserAuthApi.Services;

namespace UserAuthApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public ErrorHandlingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var logService = scope.ServiceProvider.GetRequiredService<LogService>();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logService);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, LogService logService)
        {
            context.Response.ContentType = "application/json";

            ProblemDetails problemDetails;
            var source = "ErrorHandlingMiddleware";

            // Handle specific database exceptions
            switch (exception)
            {
                case SqlException sqlEx:
                    await logService.LogExceptionAsync(sqlEx.Message, source);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Title = "Database Error",
                        Detail = "An error occurred while interacting with the database. Please try again later."
                    };
                    break;

                case MongoException mongoEx:
                    await logService.LogExceptionAsync(mongoEx.Message, source);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Title = "Database Error",
                        Detail = "An error occurred while interacting with the database. Please try again later."
                    };
                    break;

                case UnauthorizedAccessException _:
                    await logService.LogExceptionAsync(exception.Message, source);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.Unauthorized,
                        Title = "Unauthorized",
                        Detail = exception.Message
                    };
                    break;

                case KeyNotFoundException _:
                    await logService.LogExceptionAsync(exception.Message, source);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = "Not Found",
                        Detail = exception.Message
                    };
                    break;

                default:
                    await logService.LogExceptionAsync(exception.Message, source);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Title = "Internal Server Error",
                        Detail = exception.Message
                    };
                    break;
            }

            await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
        }
    }
}
