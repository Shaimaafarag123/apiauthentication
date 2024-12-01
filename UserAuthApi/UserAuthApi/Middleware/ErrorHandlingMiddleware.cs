using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // For SqlException
using MongoDB.Driver; // For MongoException
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); 
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ProblemDetails problemDetails;

        // Handle specific database exceptions
        switch (exception)
        {
            case SqlException sqlEx:
                // Log SQL exceptions with details
                _logger.LogError(sqlEx, "Database error occurred.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Database Error",
                    Detail = "An error occurred while interacting with the database. Please try again later."
                };
                break;

            case MongoException mongoEx:
                // Log MongoDB exceptions with details
                _logger.LogError(mongoEx, "MongoDB error occurred.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Database Error",
                    Detail = "An error occurred while interacting with the database. Please try again later."
                };
                break;

            case UnauthorizedAccessException _:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = "Unauthorized",
                    Detail = exception.Message
                };
                break;

            case KeyNotFoundException _:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Not Found",
                    Detail = exception.Message
                };
                break;

            default:
                _logger.LogError(exception, "Unhandled exception occurred.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Internal Server Error",
                    Detail = exception.Message
                };
                break;
        }

        return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
    }
}
