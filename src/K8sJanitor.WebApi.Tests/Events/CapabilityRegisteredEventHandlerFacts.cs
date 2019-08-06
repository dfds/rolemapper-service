using System;
using System.Linq;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Tests.TestDoubles;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Xunit;

namespace K8sJanitor.WebApi.Tests.Events
{
    public class CapabilityRegisteredEventHandlerFacts
    {
        [Fact]
        public async Task HandleAsync_Will_Use_Event_Payload_Correctly()
        {
            // Arrange

            var configMapServiceSpy = new ConfigMapServiceSpy();
            var namespaceRepositorySpy = new NamespaceRepositorySpy();
            var roleRepositorySpy = new RoleRepositorySpy();
            var roleBindingRepositorySpy = new RoleBindingRepositorySpy();
            var logger = new LoggerFactory().CreateLogger<CapabilityRegisteredEventHandler>();
            var sut = new CapabilityRegisteredEventHandler(
                configMapServiceSpy,
                namespaceRepositorySpy,
                roleRepositorySpy,
                roleBindingRepositorySpy,
                logger
            );

            var capabilityName = "capabilityName".ToLower();
            var roleArn = "rolearn";

            var data = new CapabilityRegisteredDomainEventData(capabilityName, roleArn);
            var g = new GeneralDomainEvent(
                "1",
                "capability_registered",
                Guid.NewGuid().ToString(),
                "sender",
                JObject.FromObject(data)
            );
            var @event = new CapabilityRegisteredDomainEvent(g);


            // Act
            await sut.HandleAsync(@event);


            // Assert
            Assert.Equal(capabilityName, configMapServiceSpy.Roles.Single().Key);
            Assert.Equal(roleArn, configMapServiceSpy.Roles.Single().Value);

            Assert.Equal(capabilityName, namespaceRepositorySpy.Namespaces.Single().NamespaceName);
            Assert.Equal(capabilityName, namespaceRepositorySpy.Namespaces.Single().NamespaceName);

            Assert.Equal(capabilityName, roleRepositorySpy.Namespaces.Single());

            Assert.Equal(capabilityName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item1);
            Assert.Equal(capabilityName + "-full-access-role",
                roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item2);
            Assert.Equal(capabilityName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item3);
        }
    }
}