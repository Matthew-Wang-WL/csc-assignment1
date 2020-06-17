using Microsoft.AspNetCore.Builder;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Stripe;
using Stripe.BillingPortal;
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

        public string GetBillingPortalURL(string customerId, string returnURL)
        {
            StripeConfiguration.ApiKey = stripeKey;

            //Create billing portal session
            var options = new SessionCreateOptions
            {
                Customer = customerId,
                ReturnUrl = returnURL,
            };
            var service = new SessionService();
            var response = service.Create(options);

            //return the portal url
            return response.Url;
        }


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

            var options = new PlanListOptions { Active = true };
            var service = new PlanService();
            return service.List(options);
        }

        public bool AttachPaymentMethodToCustomer(SubscriptionDetails details)
        {
            StripeConfiguration.ApiKey = stripeKey;
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
            StripeConfiguration.ApiKey = stripeKey;
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
            StripeConfiguration.ApiKey = stripeKey;
            try
            {
                SubscriptionCreateOptions options;

                if (details.unixTimestamp == 0)
                {
                    options = new SubscriptionCreateOptions
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
                } //end if
                else
                {
                    options = new SubscriptionCreateOptions
                    {
                        Customer = details.customerId,
                        Items = new List<SubscriptionItemOptions>
                        {
                            new SubscriptionItemOptions
                            {
                                 Price = details.priceId,
                            },
                        },
                        BillingCycleAnchor = DateTimeOffset.FromUnixTimeSeconds(details.unixTimestamp).UtcDateTime,
                    };

                } //end else
                var service = new SubscriptionService();

                return service.Create(options);

            } catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        public bool ChangeBillingCycle(string subscriptionId, int unixTimestamp)
        {
            StripeConfiguration.ApiKey = stripeKey;
            try
            {
                var options = new SubscriptionUpdateOptions
                {
                    TrialEnd = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime,
                    ProrationBehavior = "none",
                };
                var service = new SubscriptionService();
                service.Update(subscriptionId, options);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public StripeList<PaymentIntent> GetCustomerPaymentList(string customerId)
        {
            StripeConfiguration.ApiKey = stripeKey;
            try
            {
                var options = new PaymentIntentListOptions
                {
                    Customer = customerId,
                };
                var service = new PaymentIntentService();
                return service.List(options);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }


        public StripeList<Subscription> GetSubscriptions()
        {
            StripeConfiguration.ApiKey = stripeKey;

            var service = new SubscriptionService();
            return service.List();
        }

        public Subscription GetSubscription(string subscriptionId)
        {
            StripeConfiguration.ApiKey = stripeKey;

            var service = new SubscriptionService();
            return service.Get(subscriptionId);
        }


        public Subscription Pause(JObject data)
        {
            SubscriptionUpdateOptions options;

            if(data["resumes_at"] != null) {
                options = new SubscriptionUpdateOptions
                {
                    PauseCollection = new SubscriptionPauseCollectionOptions
                    {
                        Behavior = data["behavior"].ToString(),
                        ResumesAt = DateTimeOffset.FromUnixTimeSeconds(int.Parse(data["resumes_at"].ToString())).UtcDateTime,
                    },
                };
            } 
            else
            {
                options = new SubscriptionUpdateOptions
                {
                    PauseCollection = new SubscriptionPauseCollectionOptions
                    {
                        Behavior = data["behavior"].ToString(),
                    },
                };
            }
            
            var service = new SubscriptionService();
            return service.Update(data["subscription"].ToString(), options);
        }

        public Subscription Reactivate(JObject data)
        {
            StripeConfiguration.ApiKey = "sk_test_51GsROeLV2fzyvmGfUe7bWVg1GpdB35AqY76iPxx7PTgsV2bCFQbjxlFJBpx48i9pvCy2Av24sEEVzcMk0MN7sHHG00V8WniR6m";

            var options = new SubscriptionUpdateOptions();
            options.AddExtraParam("pause_collection", "");
            var service = new SubscriptionService();
            return service.Update(data["subscription"].ToString(), options);
        }
    }
}
