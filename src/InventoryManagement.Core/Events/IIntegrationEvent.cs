using System;

namespace InventoryManagement.Core.Events
{
    public interface IIntegrationEvent
    {
        Guid Id { get; }
        DateTime CreationDate { get; }
    }
}
