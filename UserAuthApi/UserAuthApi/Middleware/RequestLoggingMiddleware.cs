using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using System.Threading.Tasks;

namespace UserAuthApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMongoClient _mongoClient;
        private const int MaxRequestBodySize = 1024 * 1024;

        public RequestLoggingMiddleware(RequestDelegate next, IMongoClient mongoClient)
        {
            _next = next;
            _mongoClient = mongoClient;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var database = _mongoClient.GetDatabase("logs");
                var collection = database.GetCollection<BsonDocument>("requests");

                var log = new BsonDocument
                {
                    { "method", context.Request.Method },
                    { "path", context.Request.Path.ToString() },
                    { "timestamp", DateTime.UtcNow }
                };

                context.Request.EnableBuffering();

                if (context.Request.ContentLength.HasValue && context.Request.ContentLength > MaxRequestBodySize)
                {
                    log.Add("request_body", "Request body too large");
                }
                else
                {
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);

                    log.Add("request_body", requestBody);
                }

                await collection.InsertOneAsync(log);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging request: {ex.Message}");
            }

            await _next(context);
        }
    }
}
