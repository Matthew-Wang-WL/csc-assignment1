using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task6_StripePayment.Models
{
    public class InvoiceViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("InvoiceId")]
        public string InvoiceId { get; set; }
        [BsonElement("CustomerId")]
        public string CustomerId { get; set; }
        [BsonElement("SubscriptionId")]
        public string SubscriptionId { get; set; }
        [BsonElement("ChargeId")]
        public string ChargeId { get; set; }
        [BsonElement("PaymentIntentId")]
        public string PaymentIntentId { get; set; }


        [BsonElement("CustomerName")]
        public string CustomerName { get; set; }
        [BsonElement("CustomerEmail")]
        public string CustomerEmail { get; set; }


        [BsonElement("Currency")]
        public string Currency { get; set; }
        [BsonElement("AmountDue")]
        public decimal AmountDue { get; set; }
        [BsonElement("AmountPaid")]
        public decimal AmountPaid { get; set; }
        [BsonElement("AmountRemaining")]
        public decimal AmountRemaining { get; set; }
        [BsonElement("Subtotal")]
        public decimal Subtotal { get; set; }
        [BsonElement("Total")]
        public decimal Total { get; set; }

        [BsonElement("Paid")]
        public bool Paid { get; set; }
        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("Created")]
        public DateTime Created { get; set; }

    }
}
