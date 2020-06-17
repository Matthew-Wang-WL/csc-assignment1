using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Task6_StripePayment.Models;
using Task6_StripePayment.Services;

namespace Task6_StripePayment.Controllers
{
    public class StripeWebHookController : Controller
    {
        private readonly InvoiceDbService _invoiceService;
        private readonly CustomerDbService _customerService;
        private readonly SubscriptionDbService _subscriptionService;
        private readonly ChargeDbService _chargeService;

        public StripeWebHookController(InvoiceDbService invoiceService,
            CustomerDbService customerService, SubscriptionDbService subscriptionService,
            ChargeDbService chargeService)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _subscriptionService = subscriptionService;
            _chargeService = chargeService;
        }


        [HttpPost]
        [Route("api/webhooks")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                // Handle the event
                if (stripeEvent.Type == Events.InvoicePaymentSucceeded)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    // Then define and call a method to handle the successful event.
                    await HandleInvoicePaymentEvent(invoice);
                }
                else if (stripeEvent.Type == Events.InvoicePaymentFailed)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    await HandleInvoicePaymentEvent(invoice).ConfigureAwait(false);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    await HandleSubscriptionCreated(subscription);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    HandleSubscriptionUpdated(subscription);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;
                    HandleSubscriptionDeleted(subscription);
                }
                else if (stripeEvent.Type == Events.ChargeSucceeded)
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    await HandleChargeCreated(charge);
                }
                else if (stripeEvent.Type == Events.ChargeRefunded)
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    HandleChargeRefund(charge);
                }
                else if (stripeEvent.Type == Events.ChargeRefundUpdated)
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    HandleChargeUpdated(charge);
                }
                else
                {
                    // Unexpected event type
                    return BadRequest();
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Debug.WriteLine("Exception has occurred");
                return BadRequest();
            }
        }

        //// METHODS

        // -- INVOICES --

        private async Task HandleInvoicePaymentEvent(Invoice invoice)
        {
            InvoiceViewModel myInvoice = new InvoiceViewModel
            {
                InvoiceId = invoice.Id,
                CustomerId = invoice.CustomerId,
                SubscriptionId = invoice.SubscriptionId,
                ChargeId = invoice.ChargeId,
                PaymentIntentId = invoice.PaymentIntentId,

                CustomerName = invoice.CustomerName,
                CustomerEmail = invoice.CustomerEmail,

                Currency = invoice.Currency,
                AmountDue = invoice.AmountDue / 100,
                AmountPaid = invoice.AmountPaid / 100,
                AmountRemaining = invoice.AmountRemaining / 100,
                Subtotal = invoice.Subtotal / 100,
                Total = invoice.Total / 100,

                Paid = invoice.Paid,
                Status = invoice.Status,

                Created = invoice.Created,
            };

            await _invoiceService.Create(myInvoice);
        }


        // -- SUBSCRIPTIONS --

        private async Task HandleSubscriptionCreated(Subscription subscription)
        {
            SubscriptionViewModel mySubscription = new SubscriptionViewModel
            {
                SubscriptionId = subscription.Id,
                CustomerId = subscription.CustomerId,
                ProductId = subscription.Plan.ProductId,

                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                BillingCycleAnchor = subscription.BillingCycleAnchor,

                Interval = subscription.Plan.Interval,
                IntervalCount = subscription.Plan.IntervalCount,

                CancelAt = subscription.CancelAt,
                CanceledAt = subscription.CanceledAt,
                EndedAt = subscription.EndedAt,

                TrialStart = subscription.TrialStart,
                TrialEnd = subscription.TrialEnd,

                Status = subscription.Status,
                Created = subscription.Created,
            };

            await _subscriptionService.Create(mySubscription);
        }

        private void HandleSubscriptionUpdated(Subscription subscription)
        {
            SubscriptionViewModel mySubscription = new SubscriptionViewModel
            {
                SubscriptionId = subscription.Id,
                ProductId = subscription.Plan.ProductId,

                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                BillingCycleAnchor = subscription.BillingCycleAnchor,

                Interval = subscription.Plan.Interval,
                IntervalCount = subscription.Plan.IntervalCount,

                TrialStart = subscription.TrialStart,
                TrialEnd = subscription.TrialEnd,

                Status = subscription.Status,
            };

            _subscriptionService.Update(mySubscription);
        }

        private void HandleSubscriptionDeleted(Subscription subscription)
        {
            SubscriptionViewModel mySubscription = new SubscriptionViewModel
            {
                SubscriptionId = subscription.Id,

                CanceledAt = subscription.CanceledAt,
                Status = subscription.Status,
            };

            _subscriptionService.Cancel(mySubscription);
        }

        // -- CHARGES --
        private async Task HandleChargeCreated(Charge charge)
        {
            ChargeViewModel myCharge = new ChargeViewModel
            {
                ChargeId = charge.Id,
                CustomerId = charge.CustomerId,
                InvoiceId = charge.InvoiceId,
                PaymentIntentId = charge.PaymentIntentId,
                PaymentMethodId = charge.PaymentMethod,

                Amount = charge.Amount,
                AmountRefunded = charge.AmountRefunded,
                Currency = charge.Currency,

                Description = charge.Description,
                ReceiptURL = charge.ReceiptUrl,

                Paid = charge.Paid,
                Refunded = charge.Refunded,

                Status = charge.Status,
                Created = charge.Created,
            };

            await _chargeService.Create(myCharge);
        }

        private void HandleChargeRefund(Charge charge)
        {
            ChargeViewModel myCharge = new ChargeViewModel
            {
                ChargeId = charge.Id,

                Amount = charge.Amount,
                AmountRefunded = charge.AmountRefunded,

                Description = charge.Description,

                Paid = charge.Paid,
                Refunded = charge.Refunded,

                Status = charge.Status,
            };

            _chargeService.Refund(myCharge);
        }

        private void HandleChargeUpdated(Charge charge)
        {
            ChargeViewModel myCharge = new ChargeViewModel
            {
                ChargeId = charge.Id,

                Amount = charge.Amount,
                AmountRefunded = charge.AmountRefunded,

                Description = charge.Description,

                Status = charge.Status,
            };

            _chargeService.Update(myCharge);
        }
    }
}
