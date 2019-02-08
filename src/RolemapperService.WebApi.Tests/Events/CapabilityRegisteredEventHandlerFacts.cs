using System.Linq;
using System.Threading.Tasks;
using RolemapperService.WebApi.Models.ExternalEvents;
using RolemapperService.WebApi.Tests.TestDoubles;
using Xunit;

namespace RolemapperService.WebApi.Tests.Events
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
            var sut = new CapabilityRegisteredEventHandler(
                configMapServiceSpy,
                namespaceRepositorySpy,
                roleRepositorySpy,
                roleBindingRepositorySpy
            );

            var teamName = "teamname";
            var roleArn = "rolearn";
            var @event = new CapabilityRegisteredEvent(teamName, roleArn);


            // Act
            await sut.HandleAsync(@event);


            // Assert
            Assert.Equal(teamName, configMapServiceSpy.Roles.Single().Key);
            Assert.Equal(roleArn, configMapServiceSpy.Roles.Single().Value);

            Assert.Equal(teamName, namespaceRepositorySpy.Namespaces.Single().Key);
            Assert.Equal(teamName, namespaceRepositorySpy.Namespaces.Single().Value);

            Assert.Equal(teamName, roleRepositorySpy.Namespaces.Single());

            Assert.Equal(teamName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item1);
            Assert.Equal(teamName + "-full-access-role", roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item2);
            Assert.Equal(teamName,roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item3);
        }
    }
}