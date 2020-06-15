using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task6_StripePayment.Models
{
    public class SubscriptionDetails
    {
        public string customerId { get; set; }
        public string paymentMethodId { get; set; }
        public string priceId { get; set; }
    }
}
