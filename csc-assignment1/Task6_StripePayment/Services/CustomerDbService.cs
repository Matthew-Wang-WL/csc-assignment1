using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task6_StripePayment.Models;

namespace Task6_StripePayment.Services
{
    public class CustomerDbService
    {
        private readonly IMongoCollection<CustomerViewModel> customers;

        public CustomerDbService(MyDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase db = client.GetDatabase("StripeDb");
            customers = db.GetCollection<CustomerViewModel>("Customers");
        }

        public List<CustomerViewModel> Get()
        {
            return customers.Find(customer => true).ToList();
        }

        public CustomerViewModel Get(string id)
        {
            try { 
                return customers.Find(customer => customer.StripeId == id).FirstOrDefault();
            } catch(Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> Create(CustomerViewModel customer)
        {
            try
            {
                await customers.InsertOneAsync(customer).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("Created customer");
                return true;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<ReplaceOneResult> Update(string id, CustomerViewModel customerIn)
        {
            return await customers.ReplaceOneAsync(customer => customer.Id == id, customerIn);
        }
        
        public async Task<DeleteResult> Remove(CustomerViewModel customerIn)
        {
            return await customers.DeleteOneAsync(customer => customer.Id == customerIn.Id);
        }

        public async Task<DeleteResult> Remove(string id)
        {
            return await customers.DeleteOneAsync(customer => customer.Id == id);
        }


    }
}
