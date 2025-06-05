using System.Threading.Tasks;

namespace InventoryManagement.Core.Interfaces
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(string topic, T message) where T : class;
        Task SubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class;
        Task UnsubscribeAsync(string topic);
        bool IsConnected { get; }
        Task ConnectAsync();
        Task DisconnectAsync();
    }
}
