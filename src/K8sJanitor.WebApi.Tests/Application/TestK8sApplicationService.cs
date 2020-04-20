using K8sJanitor.WebApi.Application;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Tests.TestDoubles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;
using Xunit;

namespace K8sJanitor.WebApi.Tests.Application
{
    public class TestK8sApplicationService
    {
        [Theory]
        [InlineData("onprem-b6fdd382", "b6fdd382-8723-410d-a7af-5c0a1eeb0a23", "8dbbe1a3-55e8-4ac9-ae7d-01c54fdd0147")]
        public async Task can_fire_k8s_namespace_created_and_aws_arn_connected(string namespaceName, string contextId, string capabilityId)
        {
            // Arrange
            var typeName = "LovelyLittleEventType";
            var contextIdUsed = Guid.Parse(contextId);
            var capabilityIdUsed = Guid.Parse(capabilityId);
            var domainEventRegistry = new DomainEventRegistrySpy(typeName); // We need to stub
            var publishingEventsQueue = new PublishingEventsQueueSpy();
            
            var sut = new K8sApplicationService(domainEventRegistry, publishingEventsQueue);

            // Act
            await sut.FireEventK8sNamespaceCreatedAndAwsArnConnected(namespaceName, contextIdUsed, capabilityIdUsed);
            
            // Assert
            var dequedEvent = publishingEventsQueue.Queue.Dequeue();
            Assert.Equal(typeName, dequedEvent.Type);

            var obj = JsonConvert.DeserializeObject<K8sNamespaceCreatedAndAwsArnConnectedEventTestStructure>(dequedEvent.Data, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            Assert.Equal(contextIdUsed,  obj.ContextId);
            Assert.Equal(capabilityIdUsed,  obj.CapabilityId);
        }

        [Fact]
        public async Task return_expected_k8s_namespace_event_payload_json()
        {
            var evtPre = new K8sNamespaceCreatedAndAwsArnConnectedEvent("onprem-b6fdd382", Guid.Parse("b6fdd382-8723-410d-a7af-5c0a1eeb0a23"), Guid.Parse("8dbbe1a3-55e8-4ac9-ae7d-01c54fdd0147"));

            var data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            Assert.Equal(
                expected: $"{{\"namespaceName\":\"onprem-b6fdd382\",\"contextId\":\"b6fdd382-8723-410d-a7af-5c0a1eeb0a23\",\"capabilityId\":\"8dbbe1a3-55e8-4ac9-ae7d-01c54fdd0147\"}}",
                actual: data
            );
        }

        public class K8sNamespaceCreatedAndAwsArnConnectedEventTestStructure
        {
            public string NamespaceName { get; set; }
            public Guid ContextId { get; set; }
            public Guid CapabilityId { get; set; }
        }
    }
}