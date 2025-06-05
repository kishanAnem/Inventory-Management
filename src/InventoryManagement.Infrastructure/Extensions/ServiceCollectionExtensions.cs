using InventoryManagement.Core.Interfaces;
using InventoryManagement.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Redis Cache Service
            services.AddSingleton<ICacheService, RedisCacheService>();

            // Register RabbitMQ Message Broker
            services.AddSingleton<IMessageBroker, RabbitMQMessageBroker>();

            // Register Razorpay Payment Service
            services.AddScoped<IPaymentService, RazorpayService>();

            // Register WhatsApp Business Service
            services.AddHttpClient();
            services.AddScoped<IWhatsAppService, WhatsAppBusinessService>();

            return services;
        }
    }
}
