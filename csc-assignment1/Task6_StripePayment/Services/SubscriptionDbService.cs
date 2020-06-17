using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task6_StripePayment.Models;

namespace Task6_StripePayment.Services
{
    public class SubscriptionDbService
    {
        private readonly IMongoCollection<SubscriptionViewModel> subscriptions;

        public SubscriptionDbService(MyDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase db = client.GetDatabase("StripeDb");
            subscriptions = db.GetCollection<SubscriptionViewModel>("Subscriptions");
        }

        public async Task Create(SubscriptionViewModel subscription)
        {
            try
            {
                await subscriptions.InsertOneAsync(subscription).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("Created subscription");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public void Update(SubscriptionViewModel subscription)
        {
            try
            {
                var filter = Builders<SubscriptionViewModel>.Filter.Eq("SubscriptionId", subscription.SubscriptionId);
                var update = Builders<SubscriptionViewModel>.Update
                    .Set("ProductId", subscription.ProductId)
                    .Set("CurrentPeriodStart", subscription.CurrentPeriodStart)
                    .Set("CurrentPeriodEnd", subscription.CurrentPeriodEnd)
                    .Set("BillingCycleAnchor", subscription.BillingCycleAnchor)
                    .Set("Interval", subscription.Interval)
                    .Set("IntervalCount", subscription.IntervalCount)
                    .Set("TrialStart", subscription.TrialStart)
                    .Set("TrialEnd", subscription.TrialEnd)
                    .Set("Status", subscription.Status)
                    .CurrentDate("LastModified");

                subscriptions.UpdateOne(filter, update);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public void Cancel(SubscriptionViewModel subscription)
        {
            try
            {
                var filter = Builders<SubscriptionViewModel>.Filter.Eq("SubscriptionId", subscription.SubscriptionId);
                var update = Builders<SubscriptionViewModel>.Update
                    .Set("CanceledAt", subscription.CanceledAt)
                    .Set("Status", subscription.Status)
                    .CurrentDate("LastModified");

                subscriptions.UpdateOne(filter, update);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}
