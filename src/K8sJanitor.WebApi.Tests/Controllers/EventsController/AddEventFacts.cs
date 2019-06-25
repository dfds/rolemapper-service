using System.Net;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Infrastructure.Messaging;
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

                var eventRegistry = new DomainEventRegistry();
                eventRegistry.Register(
                    eventTypeName: "capability_registered",
                    topicName: "foo",
                    eventHandler: teamCreatedEventHandlerStub);


                var client = builder
                    .WithService<IDomainEventRegistry>(eventRegistry)

                    .Build();

                var input = @"{
                                    ""eventName"": ""capability_registered"",
                                    ""version"": ""1"",
                                    ""payload"": {
                                        ""capabilityName"": ""ADFS-ViewOnly"",
                                        ""roleArn"": ""arn:aws:iam::738063116313:role/ADFS-ViewOnly""
                                    }
                                }";

                var response = await client.PostAsync("/api/events", new JsonContent(input));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(teamCreatedEventHandlerStub.HandleAsyncGotCalled);
            }
        }
    }
}