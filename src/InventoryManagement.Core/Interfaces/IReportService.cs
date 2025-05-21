using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IReportService
{
    Task<object> GenerateProfitLossReportAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task<object> GenerateSalesTrendReportAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task<object> GenerateInventoryReportAsync(Guid tenantId);
    Task<object> GenerateCustomerAnalyticsAsync(Guid tenantId);
    Task<byte[]> ExportReportAsync(string reportType, object data);
    Task<object> GetDashboardMetricsAsync(Guid tenantId);
}
