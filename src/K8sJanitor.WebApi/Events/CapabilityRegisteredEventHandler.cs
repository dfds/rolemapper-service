using System.Threading.Tasks;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Models.ExternalEvents;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;

namespace K8sJanitor.WebApi
{
    public class CapabilityRegisteredEventHandler : IEventHandler<CapabilityRegisteredEvent>
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

        public async Task HandleAsync(CapabilityRegisteredEvent capabilityRegisteredEvent)
        {
            var capabilityName = capabilityRegisteredEvent.CapabilityName.ToLower();
            
            var configmapRoleName = capabilityName;
            await _configMapService.AddRole(
                roleName: configmapRoleName,
                roleArn: capabilityRegisteredEvent.RoleArn
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