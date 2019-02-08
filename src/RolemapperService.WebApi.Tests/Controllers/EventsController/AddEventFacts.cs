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
        public async Task Will_Handle_TeamCreatedEvent()
        {
            using (var builder = new HttpClientBuilder())
            {
                var teamCreatedEventHandlerStub = new TeamCreatedEventHandlerStub();
                var client = builder
                    .WithService<IEventHandler<TeamCreatedEvent>>(teamCreatedEventHandlerStub)
                    .Build();

                var input = @"{
                                    ""teamName"": ""ADFS-ViewOnly"",
                                    ""roleArn"": ""arn:aws:iam::738063116313:role/ADFS-ViewOnly"",
                                    ""roleName"": ""ADFS-ViewOnly""
                                }";

                var response = await client.PostAsync("/api/events", new JsonContent(input));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(teamCreatedEventHandlerStub.HandleAsyncGotCalled);
            }
        }
    }
}