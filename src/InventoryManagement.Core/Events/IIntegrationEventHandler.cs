using System.Threading.Tasks;

namespace InventoryManagement.Core.Events
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IIntegrationEvent
    {
        Task HandleAsync(TIntegrationEvent @event);
    }
}
