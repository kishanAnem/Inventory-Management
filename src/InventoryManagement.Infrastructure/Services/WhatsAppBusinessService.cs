using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagement.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.Infrastructure.Services
{
    public class WhatsAppBusinessService : IWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _accessToken;

        public WhatsAppBusinessService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["WhatsApp:ApiUrl"];
            _accessToken = configuration["WhatsApp:AccessToken"];
            
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
        }

        public async Task<bool> SendMessageAsync(string phoneNumber, string message)
        {
            var payload = new
            {
                messaging_product = "whatsapp",
                to = phoneNumber,
                type = "text",
                text = new { body = message }
            };

            var response = await SendRequestAsync(payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendTemplateMessageAsync(string phoneNumber, string templateName, object parameters)
        {
            var payload = new
            {
                messaging_product = "whatsapp",
                to = phoneNumber,
                type = "template",
                template = new
                {
                    name = templateName,
                    language = new { code = "en" },
                    components = parameters
                }
            };

            var response = await SendRequestAsync(payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendOrderUpdateAsync(string phoneNumber, string orderId, string status)
        {
            return await SendTemplateMessageAsync(phoneNumber, "order_update", new
            {
                type = "body",
                parameters = new[]
                {
                    new { type = "text", text = orderId },
                    new { type = "text", text = status }
                }
            });
        }

        public async Task<bool> SendPaymentConfirmationAsync(string phoneNumber, string orderId, decimal amount)
        {
            return await SendTemplateMessageAsync(phoneNumber, "payment_success", new
            {
                type = "body",
                parameters = new[]
                {
                    new { type = "text", text = orderId },
                    new { type = "text", text = amount.ToString("C") }
                }
            });
        }

        private async Task<HttpResponseMessage> SendRequestAsync(object payload)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            return await _httpClient.PostAsync(_apiUrl, content);
        }
    }
}
