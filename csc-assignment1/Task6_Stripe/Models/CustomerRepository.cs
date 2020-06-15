using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace Task6_Stripe.Models
{
    public interface ICustomerRepository
    {
        List<CustomerViewModel> Get();
        CustomerViewModel Get(string id);
        Task<bool> Create(CustomerViewModel customer);
        Task<ReplaceOneResult> Update(string id, CustomerViewModel customerIn);
        Task<DeleteResult> Remove(CustomerViewModel customerIn);
        Task<DeleteResult> Remove(string id);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<CustomerViewModel> customers;

        public CustomerRepository()
        {
            MongoClient client = new MongoClient(ConfigurationManager.ConnectionStrings["mongoDbConnectionString"].ConnectionString);
            IMongoDatabase db = client.GetDatabase("StripeDb");
            customers = db.GetCollection<CustomerViewModel>("Customers");
        }

        public List<CustomerViewModel> Get()
        {
            return customers.Find(customer => true).ToList();
        }

        public CustomerViewModel Get(string id)
        {
            return customers.Find(customer => customer.Id == id).FirstOrDefault();
        }

        public async Task<bool> Create(CustomerViewModel customer)
        {
            try
            {
                await customers.InsertOneAsync(customer).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("Created customer");
                return true;
            }
            catch (Exception e)
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