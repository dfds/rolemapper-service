using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using K8sJanitor.WebApi.Tests.EventHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace K8sJanitor.WebApi.Tests.Application
{
    public class TestK8sApplicationService
    {
        [Theory]
        [InlineData("onprem-b6fdd382", "b6fdd382-8723-410d-a7af-5c0a1eeb0a23", "8dbbe1a3-55e8-4ac9-ae7d-01c54fdd0147")]
        public async Task can_fire_k8s_namespace_created_and_aws_arn_connected(string namespaceName, string contextId, string capabilityId)
        {
            var k8sApplicationService = new StubK8sApplicationService();

            await k8sApplicationService.FireEventK8sNamespaceCreatedAndAwsArnConnected(namespaceName, Guid.Parse(contextId), Guid.Parse(capabilityId));
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
                expected: $"{{\"namespaceName\":\"onprem-b6fdd382\",\"contextId\":\"b6fdd382-8723-410d-a7af-5c0a1eeb0a23\",\"capabilityId\"\"8dbbe1a3-55e8-4ac9-ae7d-01c54fdd0147\"}}",
                actual: data
            );
        }
    }
}