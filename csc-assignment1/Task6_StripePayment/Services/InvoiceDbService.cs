using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task6_StripePayment.Models;

namespace Task6_StripePayment.Services
{
    public class InvoiceDbService
    {
        private readonly IMongoCollection<InvoiceViewModel> invoices;

        public InvoiceDbService(MyDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase db = client.GetDatabase("StripeDb");
            invoices = db.GetCollection<InvoiceViewModel>("Invoices");
        }

        public async Task Create(InvoiceViewModel invoice)
        {
            try
            {
                await invoices.InsertOneAsync(invoice).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("Created invoice");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}
