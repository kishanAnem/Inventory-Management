using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface INotificationService
{
    Task SendWhatsAppMessageAsync(string phoneNumber, string message);
    Task SendEmailAsync(string email, string subject, string body);
    Task SendSubscriptionReminderAsync(Guid tenantId);
    Task SendLowStockAlertAsync(Guid tenantId, Guid productId);
    Task SendPaymentConfirmationAsync(string transactionId);
    Task SendInvoiceAsync(Guid saleId, string customerEmail);
}
