using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task6_StripePayment.Models
{
    public class SubscriptionViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("SubscriptionId")]
        public string SubscriptionId { get; set; }
        [BsonElement("CustomerId")]
        public string CustomerId { get; set; }
        [BsonElement("ProductId")]
        public string ProductId { get; set; }


        [BsonElement("CurrentPeriodStart")]
        public DateTime CurrentPeriodStart { get; set; }
        [BsonElement("CurrentPeriodEnd")]
        public DateTime CurrentPeriodEnd { get; set; }
        [BsonElement("BillingCycleAnchor")]
        public DateTime? BillingCycleAnchor { get; set; }


        [BsonElement("Interval")]
        public string Interval { get; set; }
        [BsonElement("IntervalCount")]
        public long IntervalCount { get; set; }


        [BsonElement("CancelAt")]
        public DateTime? CancelAt { get; set; }
        [BsonElement("CanceledAt")]
        public DateTime? CanceledAt { get; set; }
        [BsonElement("EndedAt")]
        public DateTime? EndedAt { get; set; }


        [BsonElement("TrialStart")]
        public DateTime? TrialStart { get; set; }
        [BsonElement("TrialEnd")]
        public DateTime? TrialEnd { get; set; }


        [BsonElement("Status")]
        public string Status { get; set; }
        [BsonElement("Created")]
        public DateTime Created { get; set; }


        

    }
}
