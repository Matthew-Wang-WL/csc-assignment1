using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Stripe;
using Task6_StripePayment.Services;

namespace Task6_StripePayment.Controllers
{
    [Route("[controller]")]
    public class RefundsController : Controller
    {

        private readonly string stripeKey = "sk_test_51GsROeLV2fzyvmGfUe7bWVg1GpdB35AqY76iPxx7PTgsV2bCFQbjxlFJBpx48i9pvCy2Av24sEEVzcMk0MN7sHHG00V8WniR6m";
        private readonly StripeService _stripeService;

        public RefundsController(StripeService stripeService)
        {
            _stripeService = stripeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{custId}/charges")]
        public IActionResult GetPayments(string custId)
        {
            StripeList<PaymentIntent> paymentIntents = _stripeService.GetCustomerPaymentList(custId);

            if (paymentIntents != null)
            {
                return new JsonResult(paymentIntents);
            }
            else
            {
                return BadRequest("Failed to get customer payment intents");
            }
        }

        [HttpPost("refund-payment")]
        public IActionResult RefundPayment()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var jo = JObject.Parse(reader.ReadToEndAsync().Result);

                string response;
                try
                {
                    StripeConfiguration.ApiKey = stripeKey;
                    RefundCreateOptions options;

                    if (jo["amount"].ToString() != "" && jo["amount"].ToString() != null)
                    {
                        options = new RefundCreateOptions
                        {
                            PaymentIntent = jo["payment_id"].ToString(),
                            Reason = "requested_by_customer",
                            Amount = long.Parse(jo["amount"].ToString()),
                        };
                    } else
                    {
                        options = new RefundCreateOptions
                        {
                            PaymentIntent = jo["payment_id"].ToString(),
                            Reason = "requested_by_customer",
                        };
                    }
                    
                    options.AddExpand("payment_intent");

                    var service = new RefundService();
                    Refund refundResponse = service.Create(options);

                    response = JObject.FromObject(new
                    {
                        response = new
                        {
                            refund = refundResponse,
                        }
                    }).ToString();


                    return Ok(response);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    response = JObject.FromObject(new
                    {
                        response = new
                        {
                            error = new
                            {
                                code = 404,
                                message = e.Message,
                            }
                        }
                    }).ToString();
                    return BadRequest(response);
                }

            }
        }
        
    }
}
