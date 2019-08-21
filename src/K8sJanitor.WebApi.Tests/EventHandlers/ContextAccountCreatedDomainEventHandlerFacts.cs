using System;
using System.Linq;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Infrastructure.AWS;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Tests.TestDoubles;
using Microsoft.Extensions.Logging;
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
            var k8sApplicationService = new StubK8sApplicationService();
            var logger = new LoggerFactory().CreateLogger<ContextAccountCreatedDomainEventHandler>();
            var sut = new ContextAccountCreatedDomainEventHandler(
                configMapServiceSpy,
                namespaceRepositorySpy,
                roleRepositorySpy,
                roleBindingRepositorySpy,
                k8sApplicationService,
                logger
            );

            var @event = new ContextAccountCreatedDomainEventBuilder().Build();


            // Act
            await sut.HandleAsync(@event);


            // Assert
            Assert.NotEmpty(configMapServiceSpy.Roles.Single().Key);
            Assert.NotEmpty(configMapServiceSpy.Roles.Single().Value);

            var @namespace = namespaceRepositorySpy.Namespaces.Single();
            var namespaceName = @namespace.NamespaceName;

            Assert.NotNull(namespaceName);

            Assert.Equal(@event.Payload.CapabilityRootId, namespaceName);
            Assert.Equal(IAM.ConstructRoleArn(@event.Payload.AccountId, ".*"), @namespace.Annotations["iam.amazonaws.com/permitted"]);

            Assert.Equal(namespaceName, roleRepositorySpy.Namespaces.Single());

            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item1);
            Assert.Equal(namespaceName + "-full-access-role",
                roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item2);
            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item3);

        }


        [Fact]
        public async Task HandleAsync_Will_Act_Idempotent_When_Namespace_Already_Exist()
        {
            // Arrange
            var configMapServiceSpy = new ConfigMapServiceSpy();
            var namespaceRepositorySpy = new ErrornousNamespaceRepository(new NamespaceAlreadyExistException("Namespace already exist"));
            var roleRepositorySpy = new RoleRepositorySpy();
            var roleBindingRepositorySpy = new RoleBindingRepositorySpy();
            var k8sAppService = new K8sApplicationServiceSpy();
            var logger = new LoggerFactory().CreateLogger<ContextAccountCreatedDomainEventHandler>();
            var sut = new ContextAccountCreatedDomainEventHandler(
                configMapServiceSpy,
                namespaceRepositorySpy,
                roleRepositorySpy,
                roleBindingRepositorySpy,
                k8sAppService,
                logger
            );

            var @event = new ContextAccountCreatedDomainEventBuilder().Build();

            // Act
            await sut.HandleAsync(@event);

            // Assert
            Assert.NotEmpty(configMapServiceSpy.Roles.Single().Value);

            var namespaceName = configMapServiceSpy.Roles.Single().Key;

            //Assert.Equal(@event.Payload.RoleArn, @namespace.Annotations["iam.amazonaws.com/permitted"]);
            //
            Assert.Equal(namespaceName, roleRepositorySpy.Namespaces.Single());
            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item1);
            Assert.Equal(namespaceName + "-full-access-role",
              roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item2);
            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item3);
            Assert.Equal(namespaceName, k8sAppService.Payload_Namespace);
            Assert.Equal(@event.Payload.ContextId, k8sAppService.Payload_ContextId);
            Assert.Equal(@event.Payload.CapabilityId, k8sAppService.Payload_CapabilityId);

        }

        [Fact]
        public async Task HandleAsync_Will_Act_Idempotent_When_Role_Already_Exist()
        {
            // Arrange
            var roleName = "new-role-to-be-created";
            var configMapServiceSpy = new ConfigMapServiceSpy();
            var namespaceRepositorySpy = new NamespaceRepositorySpy();
            var errornousRoleRepository = new ErrornousRoleRepository(new RoleAlreadyExistException($"Role already exist", roleName));
            var roleBindingRepositorySpy = new RoleBindingRepositorySpy();
            var k8sAppService = new K8sApplicationServiceSpy();
            var logger = new LoggerFactory().CreateLogger<ContextAccountCreatedDomainEventHandler>();
            var sut = new ContextAccountCreatedDomainEventHandler(
                configMapServiceSpy,
                namespaceRepositorySpy,
                errornousRoleRepository,
                roleBindingRepositorySpy,
                k8sAppService,
                logger
            );

            var @event = new ContextAccountCreatedDomainEventBuilder().Build();

            // Act
            await sut.HandleAsync(@event);

            // Assert
            var namespaceName = configMapServiceSpy.Roles.Single().Key;

            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item1);
            Assert.Equal(roleName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item2);
            Assert.Equal(namespaceName, roleBindingRepositorySpy.NamespaceRoleToGroupBindings.Single().Item3);
            Assert.Equal(namespaceName, k8sAppService.Payload_Namespace);
            Assert.Equal(@event.Payload.ContextId, k8sAppService.Payload_ContextId);
            Assert.Equal(@event.Payload.CapabilityId, k8sAppService.Payload_CapabilityId);

        }

        [Fact]
        public async Task HandleAsync_Will_Act_Idempotent_When_RoleBinding_Already_Exist()
        {
            // Arrange
            var configMapServiceSpy = new ConfigMapServiceSpy();
            var namespaceRepositorySpy = new NamespaceRepositorySpy();
            var roleRepositorySpy = new RoleRepositorySpy();
            var roleBindingRepositorySpy = new ErrornousRoleBindingRepository(new RoleBindingAlreadyExistInNamespaceException("RoleBinding exists", "", ""));
            var k8sAppService = new K8sApplicationServiceSpy();
            var logger = new LoggerFactory().CreateLogger<ContextAccountCreatedDomainEventHandler>();
            var sut = new ContextAccountCreatedDomainEventHandler(
                configMapServiceSpy,
                namespaceRepositorySpy,
                roleRepositorySpy,
                roleBindingRepositorySpy,
                k8sAppService,
                logger
            );

            var @event = new ContextAccountCreatedDomainEventBuilder().Build();

            // Act and Assert
            await sut.HandleAsync(@event);
        }
    }




    public class ContextAccountCreatedDomainEventBuilder
    {
        public ContextAccountCreatedDomainEvent Build()
        {
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



            return new ContextAccountCreatedDomainEvent(generalDomainEvent);
        }


    }
}