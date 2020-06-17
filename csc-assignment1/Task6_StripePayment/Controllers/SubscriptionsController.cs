using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver.Core.Operations;
using Newtonsoft.Json.Linq;
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

        [Route("Manage")]
        public IActionResult Manage()
        {
            return View();
        }

        [Route("subscriptions/{subscriptionId}")]
        public IActionResult Subscription(string subscriptionId) {

            Subscription subscription = _stripeService.GetSubscription(subscriptionId);

            ViewBag.Subscription = subscription;

            return View();
        }

        [HttpPost("subscriptions/pause")]
        public IActionResult Pause()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                try
                {
                    var data = JObject.Parse(reader.ReadToEndAsync().Result);

                    Subscription subscription = _stripeService.Pause(data);

                    return Ok(subscription.ToJson());

                    
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return BadRequest("Failed to pause subscription");
                }
            }
        }

        [HttpPost("subscriptions/reactivate")]
        public IActionResult Reactivate()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                try
                {
                    var data = JObject.Parse(reader.ReadToEndAsync().Result);

                    Subscription subscription = _stripeService.Reactivate(data);

                    return Ok(subscription.ToJson());
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return BadRequest("Failed to reactivate subscription");
                }
            }
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

        [HttpGet]
        [Route("api/subscriptions/getall")]
        public IActionResult GetAll()
        {
            StripeList<Subscription> subscriptions = _stripeService.GetSubscriptions();
            List<Subscription> data = subscriptions.Data;

            return new JsonResult(data);
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
