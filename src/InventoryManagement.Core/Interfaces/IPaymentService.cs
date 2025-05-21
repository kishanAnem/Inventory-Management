using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(decimal amount, string paymentMethod, string currency = "INR");
    Task<bool> RefundPaymentAsync(string transactionId, decimal amount);
    Task<bool> InitiateSubscriptionPaymentAsync(Guid subscriptionId);
    Task<PaymentTransaction> GetTransactionByIdAsync(string transactionId);
    Task<IEnumerable<PaymentTransaction>> GetTransactionHistoryAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task<bool> ValidatePaymentStatusAsync(string transactionId);
}
