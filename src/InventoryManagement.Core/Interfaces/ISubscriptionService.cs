using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface ISubscriptionService
{
    Task<Subscription> CreateSubscriptionAsync(Guid tenantId);
    Task<bool> RenewSubscriptionAsync(Guid subscriptionId);
    Task<bool> CancelSubscriptionAsync(Guid subscriptionId);
    Task<bool> ProcessSubscriptionPaymentAsync(Guid subscriptionId);
    Task<bool> IsSubscriptionActiveAsync(Guid tenantId);
    Task SendSubscriptionReminderAsync(Guid tenantId);
}
