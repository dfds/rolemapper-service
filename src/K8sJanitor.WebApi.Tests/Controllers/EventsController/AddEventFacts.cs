using System.Net;
using System.Threading.Tasks;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Models.ExternalEvents;
using K8sJanitor.WebApi.Tests.Builders;
using K8sJanitor.WebApi.Tests.TestDoubles;
using Xunit;

namespace K8sJanitor.WebApi.Tests.Controllers.EventsController
{
    public class AddEventFacts
    {
        [Fact]
        public async Task Will_Handle_CapabilityRegisteredEvent()
        {
            using (var builder = new HttpClientBuilder())
            {
                var teamCreatedEventHandlerStub = new TeamCreatedEventHandlerStub();
                var client = builder
                    .WithService<IEventHandler<CapabilityRegisteredEvent>>(teamCreatedEventHandlerStub)
                    .Build();

                var input = @"{
                                    ""capabilityName"": ""ADFS-ViewOnly"",
                                    ""roleArn"": ""arn:aws:iam::738063116313:role/ADFS-ViewOnly""
                                }";

                var response = await client.PostAsync("/api/events", new JsonContent(input));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(teamCreatedEventHandlerStub.HandleAsyncGotCalled);
            }
        }
    }
}