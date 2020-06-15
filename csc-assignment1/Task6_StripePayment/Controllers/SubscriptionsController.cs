using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver.Core.Operations;
using Stripe;
using Task6_StripePayment.Models;
using Task6_StripePayment.Services;

namespace Task6_StripePayment.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly StripeService _stripeService;
        private readonly CustomerDbService _customerService;

        public SubscriptionsController(StripeService stripeService,
            CustomerDbService customerService)
        {
            _stripeService = stripeService;
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/subscriptions/get")]
        public IActionResult Get()
        {
            //Get Plans
            StripeList<Plan> plans = _stripeService.GetPlans();

            var plansArr = plans.ToArray().Reverse();
            return new JsonResult(plansArr);
        }

        [HttpPost]
        [Route("api/subscriptions/create")]
        public IActionResult Create([FromBody] SubscriptionDetails details)
        {

            Debug.WriteLine(details.ToJson());

            //Attach the payment method to the customer
            bool hasAttached = _stripeService.AttachPaymentMethodToCustomer(details);

            //Error checking
            if (hasAttached)
                Debug.WriteLine("Successfully attached payment method to customer");
            else
                Debug.WriteLine("Failed to attach payment method to customer.");

            // Change the default invoice settings on the customer to the new payment method
            bool hasUpdated = _stripeService.UpdateCustomerDefaultPaymentMethod(details);

            //Error checking
            if (hasUpdated)
                Debug.WriteLine("Successfully updated customer's default payment method");
            else
                Debug.WriteLine("Failed to update customer's default payment method.");

            // Create the subscription
            Subscription subscription = _stripeService.CreateSubscription(details);

            //Error checking
            if (subscription != null)
                Debug.WriteLine("Successfully created a subscription for the customer");
            else
                Debug.WriteLine("Failed to create a subscription for the customer");

            //Convert to json and send back to client
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(subscription);
            Debug.WriteLine(json);
            return Ok(json);
        }
    }
}
