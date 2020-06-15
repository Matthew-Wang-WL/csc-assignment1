using Microsoft.AspNetCore.Builder;
using Stripe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task6_StripePayment.Models;

namespace Task6_StripePayment.Services
{
    public class StripeService
    {
        private readonly string stripeKey = "sk_test_51GsROeLV2fzyvmGfUe7bWVg1GpdB35AqY76iPxx7PTgsV2bCFQbjxlFJBpx48i9pvCy2Av24sEEVzcMk0MN7sHHG00V8WniR6m";

        //Create a new Stripe Customer
        public Customer CreateCustomer(CustomerViewModel customer)
        {
            
            StripeConfiguration.ApiKey = stripeKey;

            var options = new CustomerCreateOptions
            {
                Name = customer.Name,
                Email = customer.Email,
            };
            var service = new CustomerService();
            return service.Create(options);
        }

        public StripeList<Plan> GetPlans()
        {
            StripeConfiguration.ApiKey = stripeKey;

            var service = new PlanService();
            return service.List();
        }

        public bool AttachPaymentMethodToCustomer(SubscriptionDetails details)
        {
            try
            {
                var options = new PaymentMethodAttachOptions
                {
                    Customer = details.customerId,
                };
                var service = new PaymentMethodService();
                service.Attach(
                  details.paymentMethodId,
                  options
                );

                return true;
            } catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public bool UpdateCustomerDefaultPaymentMethod(SubscriptionDetails details)
        {
            try
            {
                var options = new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = details.paymentMethodId,
                    },
                };
                var service = new CustomerService();
                service.Update(details.customerId, options);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public Subscription CreateSubscription(SubscriptionDetails details)
        {
            try
            {
                var options = new SubscriptionCreateOptions
                {
                    Customer = details.customerId,
                    Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = details.priceId,
                    },
                },
                };
                var service = new SubscriptionService();

                return service.Create(options);

            } catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
    }
}
