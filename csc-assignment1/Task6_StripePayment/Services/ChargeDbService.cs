using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task6_StripePayment.Models;

namespace Task6_StripePayment.Services
{
    public class ChargeDbService
    {
        private readonly IMongoCollection<ChargeViewModel> charges;

        public ChargeDbService(MyDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase db = client.GetDatabase("StripeDb");
            charges = db.GetCollection<ChargeViewModel>("Charges");
        }

        public async Task Create(ChargeViewModel charge)
        {
            try
            {
                await charges.InsertOneAsync(charge).ConfigureAwait(false);
                Debug.WriteLine("Created charge");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Refund(ChargeViewModel charge)
        {
            try
            {
                var filter = Builders<ChargeViewModel>.Filter.Eq("ChargeId", charge.ChargeId);
                var update = Builders<ChargeViewModel>.Update
                    .Set("Amount", charge.Amount)
                    .Set("AmountRefunded", charge.AmountRefunded)
                    .Set("Description", charge.Description)
                    .Set("Paid", charge.Paid)
                    .Set("Refunded", charge.Refunded)
                    .Set("Status", charge.Status)
                    .CurrentDate("LastModified");

                charges.UpdateOne(filter, update);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Update(ChargeViewModel charge)
        {
            try
            {
                var filter = Builders<ChargeViewModel>.Filter.Eq("ChargeId", charge.ChargeId);
                var update = Builders<ChargeViewModel>.Update
                    .Set("Amount", charge.Amount)
                    .Set("AmountRefunded", charge.AmountRefunded)
                    .Set("Description", charge.Description)
                    .Set("Status", charge.Status)
                    .CurrentDate("LastModified");

                charges.UpdateOne(filter, update);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
