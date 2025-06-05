using System.Threading.Tasks;

namespace InventoryManagement.Core.Interfaces
{
    public interface IWhatsAppService
    {
        Task<bool> SendMessageAsync(string phoneNumber, string message);
        Task<bool> SendTemplateMessageAsync(string phoneNumber, string templateName, object parameters);
        Task<bool> SendOrderUpdateAsync(string phoneNumber, string orderId, string status);
        Task<bool> SendPaymentConfirmationAsync(string phoneNumber, string orderId, decimal amount);
    }
}
