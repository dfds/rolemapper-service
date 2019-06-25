using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using K8sJanitor.WebApi.Domain.Events;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly ILogger<EventDispatcher> _logger;
        private readonly IDomainEventRegistry _eventRegistry;
        
        public EventDispatcher(
            ILogger<EventDispatcher> logger,
            IDomainEventRegistry eventRegistry)
        {
            _logger = logger;
            _eventRegistry = eventRegistry;
        }

        public async Task Send(string generalDomainEventJson)
        {
            var generalDomainEventObj = JsonConvert.DeserializeObject<GeneralDomainEvent>(generalDomainEventJson);
            await SendAsync(generalDomainEventObj);
        }

        public async Task SendAsync(GeneralDomainEvent generalDomainEvent)
        {
            var eventType = _eventRegistry.GetInstanceTypeFor(generalDomainEvent.EventName);
            dynamic domainEvent = Activator.CreateInstance(eventType, generalDomainEvent);
            dynamic handlersList = _eventRegistry.GetEventHandlersFor(domainEvent);
            
            foreach (var handler in handlersList)
            {
                await handler.HandleAsync(domainEvent);
            }
        }
    }
}