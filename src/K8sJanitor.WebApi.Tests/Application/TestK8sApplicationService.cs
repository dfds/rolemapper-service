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
        [InlineData("onprem-b6fdd382", "b6fdd382-8723-410d-a7af-5c0a1eeb0a23")]
        public async Task can_fire_k8s_namespace_created_and_aws_arn_connected(string namespaceName, string contextId)
        {
            var k8sApplicationService = new StubK8sApplicationService();

            await k8sApplicationService.FireEventK8sNamespaceCreatedAndAwsArnConnected(namespaceName, Guid.Parse(contextId));
        }

        [Fact]
        public async Task return_expected_k8s_namespace_event_payload_json()
        {
            var evtPre = new K8sNamespaceCreatedAndAwsArnConnectedEvent("onprem-b6fdd382", Guid.Parse("b6fdd382-8723-410d-a7af-5c0a1eeb0a23"));

            var data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            Assert.Equal(
                expected: $"{{\"namespaceName\":\"onprem-b6fdd382\",\"contextId\":\"b6fdd382-8723-410d-a7af-5c0a1eeb0a23\"}}",
                actual: data
            );
        }
    }
}