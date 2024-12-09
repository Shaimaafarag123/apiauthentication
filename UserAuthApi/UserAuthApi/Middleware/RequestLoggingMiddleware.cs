using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using UserAuthApi.Services;

namespace UserAuthApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private const int MaxRequestBodySize = 1024 * 1024; 
        public RequestLoggingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
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

                // Log the successful request after it has been processed
                if (context.Response.StatusCode < 400)
                {
                    await logService.LogRequestAsync($"Request to {context.Request.Method} {context.Request.Path} succeeded", "RequestLoggingMiddleware");
                }
                else
                {
                    var statusCode = context.Response.StatusCode;
                    string errorMessage = string.Empty;
                    switch (statusCode)
                    {
                        case 200:
                            errorMessage = "Succeed";
                            break;
                        case 400:
                            errorMessage = "Bad Request";
                            break;
                        case 401:
                             errorMessage = "Unauthorized";
                            break;
                        case 404:
                            errorMessage = "Not Found";
                            break;
                        case 500:
                            errorMessage = "Internal Server Error";
                            break;
                        default:
                            errorMessage = "An error occurs.";
                            break;
                    }

                    await logService.LogExceptionAsync(errorMessage, "RequestLoggingMiddleware");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                await logService.LogExceptionAsync(ex.Message, "RequestLoggingMiddleware");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    error = "An internal server error occurred.",
                    message = ex.Message
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
                throw;
            }
        }

        private async Task LogRequestAsync(HttpContext context, LogService logService)
        {
            try
            {
                var logMessage = $"Method: {context.Request.Method}, Path: {context.Request.Path}";

                context.Request.EnableBuffering();
                if (context.Request.ContentLength.HasValue && context.Request.ContentLength > MaxRequestBodySize)
                {
                    logMessage += ", Body: [Body too large]";
                }
                else
                {
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    logMessage += $", Body: {requestBody}";
                }

                await logService.LogRequestAsync(logMessage, "RequestLoggingMiddleware");
            }
            catch (Exception ex)
            {
                await logService.LogExceptionAsync(ex.Message, "RequestLoggingMiddleware");
            }
        }
    }
}
