using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace UserAuthApi.Services
{
    public class LogService
    {
                           
        private readonly IMongoDatabase  database;

        public LogService(IMongoClient mongoClient)
        {
            database = mongoClient.GetDatabase("logs");
           
        }



        private async Task LogAsync( BsonDocument doc  , string collection)
        {
            IMongoCollection<BsonDocument> LogCollection = database.GetCollection<BsonDocument>(collection);

            await LogCollection.InsertOneAsync(doc);
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

            await LogAsync(requestLog, "requests");
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
            await LogAsync(exceptionLog, "exceptions");

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

            //await _logsCollection.InsertOneAsync(log);
        }
    }
}
