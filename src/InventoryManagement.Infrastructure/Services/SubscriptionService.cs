using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IAuditService _auditService;

    public SubscriptionService(
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _auditService = auditService;
    }

    public async Task<Subscription> CreateSubscriptionAsync(
        Guid tenantId,
        string planType,
        decimal amount,
        bool autoRenew,
        string userId)
    {
        // Start a transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var subscription = new Subscription
            {
                TenantId = tenantId,
                PlanType = planType,
                Amount = amount,
                AutoRenew = autoRenew,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1), // Default to 1 month
                Status = "Pending",
                CreatedBy = userId
            };

            var repository = _unitOfWork.Repository<Subscription>();
            await repository.AddAsync(subscription);

            // Process payment
            var payment = await _paymentService.ProcessPaymentAsync(
                tenantId,
                "Subscription",
                subscription.Id,
                amount,
                "Card", // TODO: Make payment method configurable
                userId);

            if (payment.PaymentStatus == "Success")
            {
                subscription.Status = "Active";
                subscription.LastBillingDate = DateTime.UtcNow;
                subscription.NextBillingDate = subscription.EndDate;
                subscription.PaymentGatewaySubscriptionId = payment.TransactionId;
                await repository.UpdateAsync(subscription);
            }
            else
            {
                throw new InvalidOperationException("Subscription payment failed");
            }

            // Update tenant subscription dates
            var tenant = await _unitOfWork.Repository<Tenant>().GetByIdAsync(tenantId);
            if (tenant != null)
            {
                tenant.SubscriptionStartDate = subscription.StartDate;
                tenant.SubscriptionEndDate = subscription.EndDate;
                await _unitOfWork.Repository<Tenant>().UpdateAsync(tenant);
            }

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync();

            await _auditService.LogActivityAsync(
                "Subscription",
                subscription.Id,
                "Create",
                userId);

            return subscription;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> CancelSubscriptionAsync(Guid subscriptionId, string userId)
    {
        var repository = _unitOfWork.Repository<Subscription>();
        var subscription = await repository.GetByIdAsync(subscriptionId);

        if (subscription == null || subscription.Status != "Active")
        {
            return false;
        }

        subscription.Status = "Cancelled";
        subscription.AutoRenew = false;
        subscription.LastModifiedBy = userId;
        subscription.LastModifiedAt = DateTime.UtcNow;

        await repository.UpdateAsync(subscription);

        await _auditService.LogActivityAsync(
            "Subscription",
            subscriptionId,
            "Cancel",
            userId);

        return true;
    }

    public async Task<Subscription?> GetActiveSubscriptionAsync(Guid tenantId)
    {
        var repository = (ISubscriptionRepository)_unitOfWork.Repository<Subscription>();
        return await repository.GetActiveTenantSubscriptionAsync(tenantId);
    }

    public async Task<bool> IsSubscriptionActiveAsync(Guid tenantId)
    {
        var repository = (ISubscriptionRepository)_unitOfWork.Repository<Subscription>();
        return await repository.HasActiveSubscriptionAsync(tenantId);
    }

    public async Task<IEnumerable<Subscription>> GetSubscriptionHistoryAsync(Guid tenantId)
    {
        var repository = (ISubscriptionRepository)_unitOfWork.Repository<Subscription>();
        return await repository.GetSubscriptionHistoryAsync(tenantId);
    }

    public async Task<bool> RenewSubscriptionAsync(Guid subscriptionId, string userId)
    {
        var repository = _unitOfWork.Repository<Subscription>();
        var subscription = await repository.GetByIdAsync(subscriptionId);

        if (subscription == null || subscription.Status != "Active")
        {
            return false;
        }

        // Start a transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Process renewal payment
            var payment = await _paymentService.ProcessPaymentAsync(
                subscription.TenantId,
                "SubscriptionRenewal",
                subscription.Id,
                subscription.Amount,
                "Card", // TODO: Make payment method configurable
                userId);

            if (payment.PaymentStatus == "Success")
            {
                subscription.LastBillingDate = DateTime.UtcNow;
                subscription.StartDate = subscription.EndDate;
                subscription.EndDate = subscription.EndDate.AddMonths(1);
                subscription.NextBillingDate = subscription.EndDate;
                subscription.LastModifiedBy = userId;
                subscription.LastModifiedAt = DateTime.UtcNow;

                await repository.UpdateAsync(subscription);

                // Update tenant subscription dates
                var tenant = await _unitOfWork.Repository<Tenant>().GetByIdAsync(subscription.TenantId);
                if (tenant != null)
                {
                    tenant.SubscriptionEndDate = subscription.EndDate;
                    await _unitOfWork.Repository<Tenant>().UpdateAsync(tenant);
                }

                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync();

                await _auditService.LogActivityAsync(
                    "Subscription",
                    subscription.Id,
                    "Renew",
                    userId);

                return true;
            }
            else
            {
                throw new InvalidOperationException("Subscription renewal payment failed");
            }
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return false;
        }
    }
}
