using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Task6_StripePayment.Models;
using Task6_StripePayment.Services;

namespace Task6_StripePayment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StripeService _stripeService;
        private readonly CustomerDbService _customerService;
        private readonly IHttpContextAccessor _context;

        public HomeController(StripeService stripeService, CustomerDbService customerService, 
            ILogger<HomeController> logger, IHttpContextAccessor context)
        {
            _stripeService = stripeService;
            _customerService = customerService;
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("api/createbillingportal")]
        public IActionResult CreateBillingPortal([FromForm] IFormCollection data)
        {
            string customerId = data["CustomerId"].ToString();
            //Debug.WriteLine(customerId);

            //Validate if customer exists
            CustomerViewModel customer = _customerService.Get(customerId);
            if(customer != null) //customer exists
            {
                //Get return url
                var request = _context.HttpContext.Request;
                string returnURL = @"https://" + request.Host.ToString();
                //Debug.WriteLine(returnURL);

                //Get billing portal url
                string portalURL = _stripeService.GetBillingPortalURL(customerId, returnURL);
                //Debug.WriteLine(portalURL);

                //Redirect user to billing portal
                return Ok(new { 
                    PortalUrl = portalURL,
                });;
            }
            else
            {
                return BadRequest("Failed to create billing portal.");
            }
        }

        [Route("api/changebillingcycle")]
        public IActionResult ChangeBillingCycle([FromForm] IFormCollection data)
        {
            string subscriptionId = data["SubscriptionId"].ToString();
            int unixTimestamp = int.Parse(data["UnixTimestamp"]);

            if(unixTimestamp == 0)
            {
                return BadRequest(new
                {
                    message = "No date was selected."
                });
            }
            else
            {
                bool hasChanged = _stripeService.ChangeBillingCycle(subscriptionId, unixTimestamp);
                if (hasChanged)
                {
                    return Ok(new
                    {
                        message = "Billing cycle has changed."
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Failed to change billing cycle."
                    });
                }
            }
        }
    }
}
