using Inventory.Microservices.Net.Core.EventBus.Events;

namespace Inventory.Microservices.Net.Core.Api.ProductStore.IntegrationEvents.Events
{
    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class ProductChangedIntegrationEvent : IntegrationEvent
    {
        public int ProductId { get; private set; }

        public string NewName { get; private set; }

        public string OldName { get; private set; }

        public ProductChangedIntegrationEvent(int productId, string newName, string oldName)
        {
            ProductId = productId;
            NewName = newName;
            OldName = oldName;
        }

    }
}
