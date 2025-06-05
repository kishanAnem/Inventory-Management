using System.Threading.Tasks;
using InventoryManagement.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;

namespace InventoryManagement.Infrastructure.Services
{
    public class RazorpayService : IPaymentService
    {
        private readonly RazorpayClient _client;
        private readonly IConfiguration _configuration;

        public RazorpayService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new RazorpayClient(
                _configuration["Razorpay:KeyId"],
                _configuration["Razorpay:KeySecret"]
            );
        }

        public async Task<string> CreateOrderAsync(decimal amount, string currency, string receiptId)
        {
            var options = new Dictionary<string, object>
            {
                { "amount", amount * 100 }, // Razorpay expects amount in smallest currency unit
                { "currency", currency },
                { "receipt", receiptId },
                { "payment_capture", 1 }
            };

            var order = _client.Order.Create(options);
            return order["id"].ToString();
        }

        public async Task<bool> VerifyPaymentAsync(string orderId, string paymentId, string signature)
        {
            try
            {
                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_order_id", orderId },
                    { "razorpay_payment_id", paymentId },
                    { "razorpay_signature", signature }
                };

                Utils.verifyPaymentSignature(attributes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RefundAsync(string paymentId, decimal amount)
        {
            try
            {
                var refundOptions = new Dictionary<string, object>
                {
                    { "amount", amount * 100 },
                    { "speed", "normal" }
                };

                var refund = _client.Payment.Fetch(paymentId).Refund(refundOptions);
                return refund["status"].ToString().Equals("processed", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public async Task<object> GetPaymentDetailsAsync(string paymentId)
        {
            return await Task.FromResult(_client.Payment.Fetch(paymentId));
        }
    }
}
