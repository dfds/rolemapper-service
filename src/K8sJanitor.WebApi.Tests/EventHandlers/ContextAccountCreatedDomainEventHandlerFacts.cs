using System;
using System.Linq;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Application;
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
            var k8sApplicationService = new StubK8sApplicationService(); // Don't create a K8sApplicationService here. Instead create a stub
            var sut = new ContextAccountCreatedDomainEventHandler(
                configMapServiceSpy,
                namespaceRepositorySpy,
                roleRepositorySpy,
                roleBindingRepositorySpy,
                k8sApplicationService
            );

            var roleArn = "arn:aws:iam::123456789012:Role/RolePath";
            var id = Guid.NewGuid();
            var contextAccountCreatedDomainEventData = new ContextAccountCreatedDomainEventData(
                capabilityId: id,
                capabilityName: "foo",
                capabilityRootId: "foo-" + id.ToString().Substring(0, 8),
                contextId: Guid.NewGuid(),
                contextName: "baa",
                accountId: "210987654321",
                roleArn: roleArn,
                roleEmail: ""
            );

            var generalDomainEvent =
                new GeneralDomainEvent(
                    "1",
                    "eventName",
                    Guid.NewGuid().ToString(),
                    string.Empty,
                    JObject.FromObject(contextAccountCreatedDomainEventData)
                );

            var @event = new ContextAccountCreatedDomainEvent(generalDomainEvent);


            // Act
            await sut.HandleAsync(@event);


            // Assert
            Assert.NotEmpty(configMapServiceSpy.Roles.Single().Key);
            Assert.NotEmpty(configMapServiceSpy.Roles.Single().Value);

            var @namespace = namespaceRepositorySpy.Namespaces.Single();
            var namespaceName = @namespace.NamespaceName;
            
            Assert.NotNull(namespaceName);
            
            Assert.Equal(contextAccountCreatedDomainEventData.CapabilityRootId, namespaceName);
            Assert.Equal(roleArn, @namespace.Annotations["iam.amazonaws.com/permitted"]);

            Assert.Equal(namespaceName,roleRepositorySpy.Namespaces.Single());

            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item1);
            Assert.Equal(namespaceName + "-full-access-role",
                roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item2);
            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item3);
            
        }
    }
}