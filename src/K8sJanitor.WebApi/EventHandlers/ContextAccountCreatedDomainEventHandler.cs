using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;

namespace K8sJanitor.WebApi.EventHandlers
{
    public class ContextAccountCreatedDomainEventHandler  : IEventHandler<ContextAccountCreatedDomainEvent>
    {
        private readonly IConfigMapService _configMapService;
        private readonly INamespaceRepository _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;

        public ContextAccountCreatedDomainEventHandler(
            IConfigMapService configMapService,
            INamespaceRepository namespaceRepository,
            IRoleRepository roleRepository,
            IRoleBindingRepository roleBindingRepository
        )
        {
            _configMapService = configMapService;
            _namespaceRepository = namespaceRepository;
            _roleRepository = roleRepository;
            _roleBindingRepository = roleBindingRepository;
        }


        public async Task HandleAsync(ContextAccountCreatedDomainEvent domainEvent)
        {
            var namespaceName = await CreateNameSpace(domainEvent);

            await ConnectAwsArnToNameSpace(namespaceName, domainEvent.Payload.RoleArn);

            var namespaceRoleName = await _roleRepository
                .CreateNamespaceFullAccessRole(namespaceName);

            await _roleBindingRepository.BindNamespaceRoleToGroup(
                namespaceName: namespaceName,
                role: namespaceRoleName,
                group: namespaceName
            );
        }

        public async Task<NamespaceName> CreateNameSpace(ContextAccountCreatedDomainEvent domainEvent)
        {
            var namespaceName = NamespaceName.Create(domainEvent.Payload.CapabilityRootId);

            var labels = new List<Label>
            {
                Label.CreateSafely("capability-id", domainEvent.Payload.CapabilityId.ToString()),
                Label.CreateSafely("capability-name", domainEvent.Payload.CapabilityName),
                Label.CreateSafely("context-id", domainEvent.Payload.ContextId.ToString()),
                Label.CreateSafely("context-name", domainEvent.Payload.ContextName)
            };

            await _namespaceRepository.CreateNamespaceAsync(namespaceName, labels);

            return namespaceName;
        }

        public async Task ConnectAwsArnToNameSpace(NamespaceName namespaceName, string roleArn)
        {
            var roleName = namespaceName;

            await _configMapService.AddRole(
                roleName: roleName,
                roleArn: roleArn
            );
            var annotations = new Dictionary<string, string> {{"iam.amazonaws.com/permitted", roleName}};
            await _namespaceRepository.AddAnnotations(namespaceName, annotations);
        } 
    }
}