using System.Net;
using System.Threading.Tasks;
using RolemapperService.WebApi.Models.ExternalEvents;
using RolemapperService.WebApi.Tests.Builders;
using RolemapperService.WebApi.Tests.TestDoubles;
using Xunit;

namespace RolemapperService.WebApi.Tests.Controllers.EventsController
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