using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace K8sJanitor.WebApi.Application
{
    public class K8sApplicationService : IK8sApplicationService
    {
        private readonly IDomainEventRegistry _domainEventRegistry;
        private readonly IPublishingEventsQueue _publishingEventsQueue;


        public K8sApplicationService(IDomainEventRegistry domainEventRegistry, IPublishingEventsQueue publishingEventsQueue)
        {
            _domainEventRegistry = domainEventRegistry;
            _publishingEventsQueue = publishingEventsQueue;
        }

        public async Task FireEventK8sNamespaceCreatedAndAwsArnConnected(string namespaceName, Guid contextId, Guid capabilityId)
        {
            try
            {
                var evtPre = new K8sNamespaceCreatedAndAwsArnConnectedEvent(namespaceName, contextId, capabilityId);
            
                var evt = new DomainEventEnvelope
                {
                    EventId = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    Type = _domainEventRegistry.GetTypeNameFor(evtPre),
                    Format = "application/json",
                    Data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                };
            
                // TODO JFHEI : Need to make this async
                _publishingEventsQueue.AddEventToQueue(evt);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to emit an event from K8sApplicationService: {ex.Message}");
            }
        }
    }
}