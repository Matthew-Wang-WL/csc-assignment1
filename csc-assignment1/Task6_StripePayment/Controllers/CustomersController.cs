using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Task6_StripePayment.Models;
using Task6_StripePayment.Services;
using System.Diagnostics;

namespace Task6_StripePayment.Controllers
{
    public class CustomersController : Controller
    {
        private readonly StripeService _stripeService;
        private readonly CustomerDbService _customerService;

        public CustomersController(StripeService stripeService,
            CustomerDbService customerService)
        {
            _stripeService = stripeService;
            _customerService = customerService;
        }

        // GET: CustomersController
        public ActionResult Index()
        {
            return View();
        }

        // POST - Create a new Stripe customer and a new internal customer account
        [HttpPost]
        [Route("api/customers/create")]
        public IActionResult Create([FromForm] IFormCollection data)
        {
            //Create new customer object
            CustomerViewModel customer = new CustomerViewModel
            {
                //Extract data
                Name = data["Name"].ToString(),
                Email = data["Email"].ToString(),
            };

            // Create new customer, get the id and assign to internal customer object
            Customer stripeCustomer = _stripeService.CreateCustomer(customer);
            customer.StripeId = stripeCustomer.Id;

            //Create new customer document in database
            bool docCreated = _customerService.Create(customer).Result;
            if (docCreated)
            {
                return new JsonResult(new
                {
                    customerId = customer.StripeId
                });
            }
            else
            {
                return BadRequest("Failed to create account");
            }

        }
    }
}
