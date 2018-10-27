using Inventory.Microservices.Net.Core.Api.Base.IntegrationEvents.Events;
using Inventory.Microservices.Net.Core.EventBus.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Microservices.Net.Core.Api.Base.IntegrationEvents.EventHandling
{
    public class ProductChangedIntegrationEventHandler : IIntegrationEventHandler<ProductChangedIntegrationEvent>
    {
        //private readonly IBasketRepository _repository;

        //public ProductChangedIntegrationEventHandler(IBasketRepository repository)
        //{
        //    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        //}

        public async Task Handle(ProductChangedIntegrationEvent @event)
        {
            var userIds = @event.NewName;

            //var test = Convert.ToInt32(userIds);
            
            //foreach (var id in userIds)
            //{
            //    var basket = await _repository.GetBasketAsync(id);

            //    await UpdatePriceInBasketItems(@event.ProductId, @event.NewPrice, @event.OldPrice, basket);                      
            //}
        }
    }
}

