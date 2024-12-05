using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace UserAuthApi.Services
{
    public class LogService
    {
        private readonly IMongoCollection<BsonDocument> _logsCollection;
        private readonly IMongoCollection<BsonDocument> _exceptionsCollection;
        private readonly IMongoCollection<BsonDocument> _requestsCollection; 

        public LogService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("logs");
            _logsCollection = database.GetCollection<BsonDocument>("logs");
            _exceptionsCollection = database.GetCollection<BsonDocument>("exceptions");
            _requestsCollection = database.GetCollection<BsonDocument>("requests"); // 
        }

        // Method to log successful requests
        public async Task LogRequestAsync(string message, string source)
        {
            var requestLog = new BsonDocument
            {
                { "message", message },
                { "source", source },
                { "timestamp", DateTime.UtcNow }
            };

            await _requestsCollection.InsertOneAsync(requestLog);
        }

        // Method to log exceptions
        public async Task LogExceptionAsync(string message, string source)
        {
            var exceptionLog = new BsonDocument
            {
                { "message", message },
                { "source", source },
                { "timestamp", DateTime.UtcNow }
            };

            await _logsCollection.InsertOneAsync(exceptionLog);
        }

        // Method to log general messages (can be used for other types of logs)
        public async Task LogMessageAsync(string message, string source)
        {
            var log = new BsonDocument
            {
                { "message", message },
                { "source", source },
                { "timestamp", DateTime.UtcNow }
            };

            await _logsCollection.InsertOneAsync(log);
        }
    }
}
