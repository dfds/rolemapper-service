using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class Outbox
    {
        private readonly DomainEventEnvelopeRepository _repository;
        private readonly IDomainEventRegistry _eventRegistry;

        public Outbox(DomainEventEnvelopeRepository repository, IDomainEventRegistry eventRegistry)
        {
            _repository = repository;
            _eventRegistry = eventRegistry;
        }

        public Task QueueDomainEvents(IAggregateDomainEvents aggregate)
        {
            var domainEvents = aggregate
                .DomainEvents
                .Select(@event => new DomainEventEnvelope
                {
                    EventId = Guid.NewGuid(),
                    AggregateId = aggregate.GetAggregateId(),
                    Created = DateTime.UtcNow,
                    Type = _eventRegistry.GetTypeNameFor(@event),
                    Format = "application/json",
                    Data = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                })
                .ToArray();

            return QueueDomainEvents(domainEvents);
        }

        public async Task QueueDomainEvents(IEnumerable<DomainEventEnvelope> domainEvents)
        {
            await _repository.Add(domainEvents);
        }
    }
}