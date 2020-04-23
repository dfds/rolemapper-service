using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using K8sJanitor.WebApi.Tests.Builders;
using K8sJanitor.WebApi.Tests.TestDoubles;
using Moq.AutoMock;
using Xunit;

namespace K8sJanitor.WebApi.Tests.Controllers.EventsController
{
    public class AddEventFacts
    {
        [Fact]
        public async Task Will_Handle_CapabilityRegisteredEvent()
        {
            //Arrange
            using var builder = new HttpClientBuilder();

            var teamCreatedEventHandlerStub = new TeamCreatedEventHandlerStub();
            var handlers = new List<IEventHandler>() { teamCreatedEventHandlerStub };
            var eventRegistry = new DomainEventRegistry(new Lazy<IEnumerable<IEventHandler>>(handlers));
            
            eventRegistry.Register<CapabilityRegisteredDomainEvent>(eventTypeName: "capability_registered", topicName: "foo");

            using var client = builder
                .WithService<IDomainEventRegistry>(eventRegistry)
                .Build();

            //Act
            var input = @"{
                                    ""eventName"": ""capability_registered"",
                                    ""version"": ""1"",
                                    ""payload"": {
                                        ""capabilityName"": ""ADFS-ViewOnly"",
                                        ""roleArn"": ""arn:aws:iam::738063116313:role/ADFS-ViewOnly""
                                    }
                                }";

            var content = new JsonContent(input);
            var response = await client.PostAsync("/api/events", content);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(teamCreatedEventHandlerStub.HandleAsyncGotCalled);
        }
    }
}