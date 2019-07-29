using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace K8sJanitor.WebApi.IntegrationTests.Kafka
{
    public class ManualEvents
    {
        // This must be manually updated whenever an Event has been added or removed.
        // Look to Startup.cs in the ConfigureDomainEvents function to see what events are currently registered.
        public static void RegisterEvents(DomainEventRegistry evtReg, string topic, IServiceScope serviceScope)
        {
            // K8sNamespaceCreatedAndAwsArnConnectedEvent
            evtReg.Register(
                eventTypeName: "k8s_namespace_created_and_aws_arn_connected",
                topicName: topic,
                eventHandler: serviceScope.ServiceProvider.GetRequiredService<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>>());
            
            // ContextAccountCreatedDomainEvent
            evtReg.Register<ContextAccountCreatedDomainEvent>(
                eventTypeName: "aws_context_account_created",
                topicName: topic,
                eventHandler: serviceScope.ServiceProvider.GetRequiredService<IEventHandler<ContextAccountCreatedDomainEvent>>() );

            // CapabilityRegisteredDomainEvent
            evtReg.Register(
                eventTypeName: "capability_registered",
                topicName: topic,
                eventHandler: serviceScope.ServiceProvider.GetRequiredService<IEventHandler<CapabilityRegisteredDomainEvent>>() );
        }

        // This must be manually updated whenever an Event has been added or removed.
        public static void AddEventsToServiceProvider(IServiceCollection serviceCollection)
        {
            serviceCollection
                // K8sNamespaceCreatedAndAwsArnConnectedEvent
                .AddTransient<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>, GenericEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>>()
                // ContextAccountCreatedDomainEvent
                .AddTransient<IEventHandler<ContextAccountCreatedDomainEvent>, GenericEventHandler<ContextAccountCreatedDomainEvent>>()
                // CapabilityRegisteredDomainEvent
                .AddTransient<IEventHandler<CapabilityRegisteredDomainEvent>, GenericEventHandler<CapabilityRegisteredDomainEvent>>();
        }

        
        // This must be manually updated whenever an Event has been added or removed.
        public static async Task AddEventsToPublishingQueue(IServiceScope scope)
        {
            // K8sNamespaceCreatedAndAwsArnConnectedEvent
            {
                var evtPre = new K8sNamespaceCreatedAndAwsArnConnectedEvent("kafkaTest", Guid.Parse("f8bbe9e1-cdda-41fb-9781-bf43dbc18a47"), Guid.Parse("2a70d5ac-5e1f-4e1d-8d81-4c4cbda7b9d9"));
                var evt = new DomainEventEnvelope
                {
                    EventId = Guid.Parse("9f780fd2-0c09-4374-8bd8-be1efb5b92ae"),
                    Created = DateTime.UtcNow,
                    Type = scope.ServiceProvider.GetRequiredService<DomainEventRegistry>().GetTypeNameFor(evtPre),
                    Format = "application/json",
                    AggregateId = Guid.Parse("d814c1af-7ead-4b76-bc43-c052cfdf09b1").ToString(),
                    Data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                };
                scope.ServiceProvider.GetRequiredService<PublishingEventsQueue>().AddEventToQueue(evt);                
            }

            // ContextAccountCreatedDomainEvent
            // This Event isn't meant to be published from this service, but it will nonetheless be tested.
            {
                var evtPre = new ContextAccountCreatedDomainEvent(new GeneralDomainEvent(
                    version: "0",
                    eventName: "aws_context_account_created",
                    xCorrelationId: "",
                    xSender: "KafkaTest",
                    payload: new ContextAccountCreatedDomainEventData(
                        capabilityId: Guid.NewGuid(),
                        capabilityName: "KafkaTestCapability",
                        capabilityRootId: "",
                        contextId: Guid.NewGuid(),
                        contextName: "KafkaTestContext",
                        accountId: "",
                        roleArn: "",
                        roleEmail: "")
                    )
                );
                
                var evt = new DomainEventEnvelope
                {
                    EventId = Guid.Parse("f34740f6-c445-4aa7-ba2d-15f7abc634e1"),
                    Created = DateTime.UtcNow,
                    Type = "aws_context_account_created",
                    Format = "application/json",
                    AggregateId = Guid.Parse("abf8c3aa-0339-4cfc-a427-7f4e67297340").ToString(),
                    Data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                };
                
                scope.ServiceProvider.GetRequiredService<PublishingEventsQueue>().AddEventToQueue(evt);
            }

            // CapabilityRegisteredDomainEvent
            // This Event isn't meant to be published from this service, but it will nonetheless be tested.
            {
                var evtPre = new CapabilityRegisteredDomainEvent(new GeneralDomainEvent(
                        version: "0",
                        eventName: "capability_registered",
                        xCorrelationId: "",
                        xSender: "KafkaTest",
                        payload: new CapabilityRegisteredDomainEventData(
                            capabilityName: "KafkaTestCapability",
                            roleArn: "")
                    )
                );
                
                var evt = new DomainEventEnvelope
                {
                    EventId = Guid.Parse("6f3299a2-5fc0-447f-bcef-85cb96b1e81e"),
                    Created = DateTime.UtcNow,
                    Type = "capability_registered",
                    Format = "application/json",
                    AggregateId = Guid.Parse("0e515da2-f175-4911-89b7-806ca67db1c4").ToString(),
                    Data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                };
                
                scope.ServiceProvider.GetRequiredService<PublishingEventsQueue>().AddEventToQueue(evt);
            }
            

        }
    }
}