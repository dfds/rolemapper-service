using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;

namespace K8sJanitor.WebApi
{
    public class CapabilityRegisteredEventHandler : IEventHandler<CapabilityRegisteredDomainEvent>
    {
        private readonly IConfigMapService _configMapService;
        private readonly INamespaceRepository _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;

        public CapabilityRegisteredEventHandler(
            IConfigMapService configMapService,
            INamespaceRepository namespaceRepository,
            IRoleRepository roleRepository,
            IRoleBindingRepository roleBindingRepository
        )
        {
            _namespaceRepository = namespaceRepository;
            _roleRepository = roleRepository;
            _roleBindingRepository = roleBindingRepository;
            _configMapService = configMapService;
        }

        public async Task HandleAsync(CapabilityRegisteredDomainEvent capabilityRegisteredDomainEvent)
        {
            var capabilityName = capabilityRegisteredDomainEvent.Payload.CapabilityName.ToLower();
            
            var configmapRoleName = capabilityName;
            await _configMapService.AddRole(
                roleName: configmapRoleName,
                roleArn: capabilityRegisteredDomainEvent.Payload.RoleArn
            );

            var namespaceName = capabilityName;

            await _namespaceRepository.CreateNamespaceAsync(
                namespaceName: namespaceName,
                roleName: configmapRoleName
            );

            var namespaceRoleName = await _roleRepository
                .CreateNamespaceFullAccessRole(namespaceName);

            await _roleBindingRepository.BindNamespaceRoleToGroup(
                namespaceName: namespaceName,
                role: namespaceRoleName,
                group: capabilityName
            );
        }
    }
}