using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests.Kafka
{
    public class KafkaTest
    {
        [Fact]
        public async Task ProduceEventAndCheckExternallyThatItHasBeenReceived()
        {
            var serviceProvider = Helper.SetupServiceProviderWithConsumerAndProducer();
            await Helper.ResetFakeServer(serviceProvider.CreateScope());
            var eventRegistry = serviceProvider.GetRequiredService<DomainEventRegistry>();

            const string topic = "build.capabilities";

            eventRegistry.Register(
                eventTypeName: "k8s_namespace_created_and_aws_arn_connected",
                topicName: topic,
                eventHandler: serviceProvider.GetRequiredService<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>>());

            var consumer = Helper.SetupKafkaConsumption(serviceProvider.CreateScope());
            
            var apiCallsReceived = await Helper.CallFakeServer("/api-calls-received", serviceProvider.CreateScope());
            Assert.Equal(1, apiCallsReceived.ApiCallsReceived);

            var consumerTask = Task.Run(() =>
            {
                var msg = consumer.Consume();
                return msg;
            });
            
            Thread.Sleep(5000);

            using (var scope = serviceProvider.CreateScope())
            {
                var evtPre = new K8sNamespaceCreatedAndAwsArnConnectedEvent("kafkaTest", Guid.Parse("f8bbe9e1-cdda-41fb-9781-bf43dbc18a47"), Guid.Parse("2a70d5ac-5e1f-4e1d-8d81-4c4cbda7b9d9"));
                var evt = new DomainEventEnvelope
                {
                    EventId = Guid.Parse("9f780fd2-0c09-4374-8bd8-be1efb5b92ae"),
                    Created = DateTime.UtcNow,
                    Type = eventRegistry.GetTypeNameFor(evtPre),
                    Format = "application/json",
                    AggregateId = Guid.Parse("d814c1af-7ead-4b76-bc43-c052cfdf09b1").ToString(),
                    Data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
                };
            
                scope.ServiceProvider.GetRequiredService<PublishingEventsQueue>().AddEventToQueue(evt);

                await Helper.RunManualPublishingServiceOnce(scope);
            }
            
            Assert.Equal(
                expected: "{\"version\":\"1\",\"eventName\":\"k8s_namespace_created_and_aws_arn_connected\",\"x-correlationId\":\"\",\"x-sender\":\"K8sJanitor.WebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"payload\":{\"namespaceName\":\"kafkaTest\",\"contextId\":\"f8bbe9e1-cdda-41fb-9781-bf43dbc18a47\",\"capabilityId\":\"2a70d5ac-5e1f-4e1d-8d81-4c4cbda7b9d9\"}}", 
                actual: consumerTask.Result.Value);

            var res = Helper.CallFakeServer("/api-calls-received", serviceProvider.CreateScope()).Result;
            Assert.Equal(res.ApiCallsReceived, apiCallsReceived.ApiCallsReceived + 1);
            Assert.Equal(1, res.KafkaMessageReceived);

            await Helper.CallFakeServer("/api-calls-reset", serviceProvider.CreateScope());
        }

        // Checks if there is any changes in events, either removed or added. This is mostly a check to make sure that
        // the removal or addition of an Event is intended. Assuming it is, one just has to update the list below in order for the test
        // to go through.
        [Fact]
        public async Task CheckExpectedEventsExists()
        {
            var events = await Helper.GetAllEventTypes(); // Looks for classes that implements IEvent or IDomainEvent.

            // This list is to be updated when an Event is either added or removed.
            var expectedEvents = new List<Type>
            {
                typeof(CapabilityRegisteredDomainEvent),
                typeof(ContextAccountCreatedDomainEvent),
                typeof(K8sNamespaceCreatedAndAwsArnConnectedEvent)
            };

            var eventsExists = true;
            foreach (var evt in events)
            {
                if (!expectedEvents.Contains(evt))
                {
                    eventsExists = false;
                }
            }

            Assert.True(eventsExists);
            Assert.Equal(events.Count, expectedEvents.Count);
        }

        // Must be manually updated to support a new Event, as well as when removing an Event.
        // That means that this test is SUPPOSED TO FAIL if one has added, removed or changed an Event.
        // TODO: Potentially use reflection to instead of doing the manual steps.
        // Take a look at ManualEvents.cs for more info.
        [Fact]
        public async Task ProduceExistingEvents()
        {
            var events = await Helper.GetAllEventTypes();
            
            var serviceProvider = Helper.SetupServiceProviderWithConsumerAndProducer(useManualEvents: true);
            await Helper.ResetFakeServer(serviceProvider.CreateScope());
            var eventRegistry = serviceProvider.GetRequiredService<DomainEventRegistry>();

            const string topic = "build.capabilities";

            using (var scope = serviceProvider.CreateScope())
            {
                ManualEvents.RegisterEvents(eventRegistry, topic, scope);
            }
            

            var apiCallsReceived = await Helper.CallFakeServer("/api-calls-received", serviceProvider.CreateScope());
            Assert.Equal(1, apiCallsReceived.ApiCallsReceived);

            using (var scope = serviceProvider.CreateScope())
            {
                await ManualEvents.AddEventsToPublishingQueue(scope);
                await Helper.RunManualPublishingServiceOnce(scope);
            }
            
            Thread.Sleep(3000);
            
            var res = Helper.CallFakeServer("/api-calls-received", serviceProvider.CreateScope()).Result;
            Assert.Equal(res.ApiCallsReceived, apiCallsReceived.ApiCallsReceived + 1);
            Assert.Equal(events.Count, res.KafkaMessageReceived);
            await Helper.CallFakeServer("/api-calls-reset", serviceProvider.CreateScope());
        }
    }
}