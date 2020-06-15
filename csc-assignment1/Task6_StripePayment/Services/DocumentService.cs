using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Task6_StripePayment.Models;

namespace Task6_StripePayment.Services
{
    public class DocumentService
    {
        private readonly MongoClient _client;
        private Dictionary<string, List<string>> _databasesAndCollections;
        private readonly string databaseName = "";

        public DocumentService(MyDatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
        }
        /*
        public async Task<BsonDocument> GetDocument(string databaseName, string collectionName, int index)
        {
            var collection = GetCollection(databaseName, collectionName);
            BsonDocument document = null;
            await collection.Find(doc => true)
              .Skip(index)
              .Limit(1)
              .ForEachAsync(doc => document = doc);
            return document;
        }
        
        private IMongoCollection<BsonDocument> GetCollection(string databaseName, string collectionName)
        {
            var db = _client.GetDatabase(databaseName);
            return db.GetCollection<BsonDocument>(collectionName);
        }

        public async Task<UpdateResult> CreateOrUpdateField(string databaseName, string collectionName, string id, string fieldName, string value)
        {
            var collection = GetCollection(databaseName, collectionName);
            var update = Builders<BsonDocument>.Update.Set(fieldName, new BsonString(value));
            return await collection.UpdateOneAsync(CreateIdFilter(id), update);
        }

        public async Task<DeleteResult> DeleteDocument(string databaseName, string collectionName, string id)
        {
            var collection = GetCollection(databaseName, collectionName);
            return await collection.DeleteOneAsync(CreateIdFilter(id));
        }

        private static BsonDocument CreateIdFilter(string id)
        {
            return new BsonDocument("_id", new BsonObjectId(new ObjectId(id)));
        }

        public async Task CreateDocument(string databaseName, string collectionName)
        {
            var collection = GetCollection(databaseName, collectionName);
            await collection.InsertOneAsync(new BsonDocument());
        }*/

        public async Task CreateCustomerDocument(CustomerViewModel customer)
        {
            var db = _client.GetDatabase(databaseName);
            var customerCollection = db.GetCollection<CustomerViewModel>("CustomerInformationCollection");
            await customerCollection.InsertOneAsync(customer);
        }
    
    }
}