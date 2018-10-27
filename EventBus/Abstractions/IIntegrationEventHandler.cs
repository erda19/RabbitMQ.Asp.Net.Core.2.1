using Inventory.Microservices.Net.Core.EventBus.Events;
using System.Threading.Tasks;

namespace Inventory.Microservices.Net.Core.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler 
        where TIntegrationEvent: IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
