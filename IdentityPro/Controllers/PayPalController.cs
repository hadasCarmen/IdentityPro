using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using PayPal.Api;
using System.Collections.Generic;
using System.Configuration;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using PayPal.OpenIdConnect;
using Payer = PayPal.Api.Payer;

namespace IdentityPro.Controllers
{
    public partial class PaymentWithPayPal : Controller
    {
        private readonly IConfiguration _configuration;

        public PaymentWithPayPal(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(decimal orderAmount)
        {
            // Your checkout logic to get the amount

            // Initialize PayPal API context
            var apiContext = new APIContext(new OAuthTokenCredential(
                _configuration["PayPal:ClientId"],
                _configuration["PayPal:ClientSecret"]
            ).GetAccessToken());

            // Create payment
            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount
                    {
                        currency = "USD",
                        total = orderAmount.ToString("F2")
					},
                    description = "Your payment description"
                }
            },
                redirect_urls = new RedirectUrls
                {
                    //return_url = "https://example.com/return",
                    //cancel_url = "https://example.com/cancel"
                }
            };

            var createdPayment = payment.Create(apiContext);

            // Redirect user to PayPal for approval
            var approvalUrl = createdPayment.links.FirstOrDefault(link => link.rel.Equals("approval_url"))?.href;
            return Redirect(approvalUrl);
        }
    }
}
//{
//    public class PayPalController : Controller
//    {
//        private readonly PayPalHttpClient _payPalClient;
//        private readonly string _clientId = "YOUR_CLIENT_ID";
//        private readonly string _clientSecret = "YOUR_CLIENT_SECRET";

//        public PayPalController()
//        {
//            var environment = new SandboxEnvironment(_clientId, _clientSecret);
//            _payPalClient = new PayPalHttpClient(environment);
//        }

//        public IActionResult Index()
//        {
//            // Display a page with a PayPal button for the user to initiate a payment
//            return View();
//        }

//        [HttpPost]
//        public IActionResult CreateOrder()
//        {
//            // Implement logic to create a PayPal order
//            var request = new OrdersCreateRequest();
//            // Set the order details, items, and other information

//            var response = _payPalClient.Execute(request);

//            // Handle the response and retrieve the order ID
//            //var orderId = response.Result<PayPalHttp.HttpResponse>().Result<PayPalCheckoutSdk.Orders.Order>().Id;

//            // Redirect the user to the PayPal approval page
//            //var approvalUrl = response.Result<PayPalHttp.HttpResponse>().Result<PayPalCheckoutSdk.Orders.Order>().Links.Find(link => link.Rel.Equals("approve")).Href;
//            return Redirect(approvalUrl);
//        }


//        [HttpGet]
//        public IActionResult CaptureOrder(string orderId)
//        {
//            // Implement logic to capture the PayPal order after user approval

//            var request = new OrdersCaptureRequest(orderId);
//            // Set any additional capture details if needed

//            var response = _payPalClient.Execute(request);

//            // Handle the response and update your system accordingly

//            return View("OrderConfirmation");
//        }
//    }
//}
