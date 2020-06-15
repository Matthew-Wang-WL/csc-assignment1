using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Task6_StripePayment.Models
{
    public class CustomerViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("StripeId")]
        public string StripeId { get; set; }
        [BsonElement("Name")]
        [Required]
        public string Name { get; set; }
        [BsonElement("Email")]
        [Required]
        public string Email { get; set; }
    }
}
