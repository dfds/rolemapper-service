using System;
using System.Linq;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Tests.TestDoubles;
using Newtonsoft.Json.Linq;
using Xunit;

namespace K8sJanitor.WebApi.Tests.EventHandlers
{
    public class ContextAccountCreatedDomainEventHandlerFacts
    {
        [Fact]
        public async Task HandleAsync_Will_Use_Event_Payload_Correctly()
        {
            // Arrange
            var configMapServiceSpy = new ConfigMapServiceSpy();
            var namespaceRepositorySpy = new NamespaceRepositorySpy();
            var roleRepositorySpy = new RoleRepositorySpy();
            var roleBindingRepositorySpy = new RoleBindingRepositorySpy();
            var sut = new ContextAccountCreatedDomainEventHandler(
                configMapServiceSpy,
                namespaceRepositorySpy,
                roleRepositorySpy,
                roleBindingRepositorySpy
            );

            var roleArn = "arn:aws:iam::123456789012:Role/RolePath";
            var contextAccountCreatedDomainEventData = new ContextAccountCreatedDomainEventData(
                capabilityId: Guid.NewGuid(),
                capabilityName: "foo",
                contextId: Guid.NewGuid(),
                contextName: "baa",
                accountId: "210987654321",
                roleArn: roleArn,
                roleEmail: ""
            );

            var generalDomainEvent =
                new GeneralDomainEvent(
                    Guid.NewGuid(),
                    string.Empty,
                    JObject.FromObject(contextAccountCreatedDomainEventData)
                );

            var @event = new ContextAccountCreatedDomainEvent(generalDomainEvent);


            // Act
            await sut.HandleAsync(@event);


            // Assert
            Assert.NotEmpty(configMapServiceSpy.Roles.Single().Key);
            Assert.NotEmpty(configMapServiceSpy.Roles.Single().Value);

            var namespaceName = namespaceRepositorySpy.Namespaces.Single().NamespaceName;
            Assert.NotNull(namespaceName);

            Assert.Equal(namespaceName,roleRepositorySpy.Namespaces.Single());

            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item1);
            Assert.Equal(namespaceName + "-full-access-role",
                roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item2);
            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item3);
        }
    }
}