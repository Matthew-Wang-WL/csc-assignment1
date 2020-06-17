using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task6_StripePayment.Models
{
    public class ChargeViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("ChargeId")]
        public string ChargeId { get; set; }
        [BsonElement("CustomerId")]
        public string CustomerId { get; set; }
        [BsonElement("InvoiceId")]
        public string InvoiceId { get; set; }
        [BsonElement("PaymentIntentId")]
        public string PaymentIntentId { get; set; }
        [BsonElement("PaymentMethodId")]
        public string PaymentMethodId { get; set; }


        [BsonElement("Amount")]
        public decimal Amount { get; set; }
        [BsonElement("AmountRefunded")]
        public decimal AmountRefunded { get; set; }
        [BsonElement("Currency")]
        public string Currency { get; set; }


        [BsonElement("Description")]
        public string Description { get; set; }
        [BsonElement("ReceiptURL")]
        public string ReceiptURL { get; set; }


        [BsonElement("Paid")]
        public bool Paid { get; set; }        
        [BsonElement("Refunded")]
        public bool Refunded { get; set; }
        
        
        [BsonElement("Status")]
        public string Status { get; set; }
        [BsonElement("Created")]
        public DateTime Created { get; set; }


    }
}
