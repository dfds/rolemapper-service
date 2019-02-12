using System.Threading.Tasks;
using RolemapperService.WebApi.Models.ExternalEvents;
using RolemapperService.WebApi.Repositories.Kubernetes;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi
{
    public class CapabilityRegisteredEventHandler : IEventHandler<CapabilityRegisteredEvent>
    {
        private readonly IConfigMapService _configMapService;
        private readonly INamespaceRespoitory _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;

        public CapabilityRegisteredEventHandler(
            IConfigMapService configMapService,
            INamespaceRespoitory namespaceRepository,
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
            capabilityRegisteredEvent.CapabilityName = capabilityRegisteredEvent.CapabilityName.ToLower();
            
            var configmapRoleName = capabilityRegisteredEvent.CapabilityName;
            await _configMapService.AddRole(
                roleName: configmapRoleName,
                roleArn: capabilityRegisteredEvent.RoleArn
            );

            var namespaceName = capabilityRegisteredEvent.CapabilityName;

            await _namespaceRepository.CreateNamespace(
                namespaceName: namespaceName,
                roleName: configmapRoleName
            );

            var namespaceRoleName = await _roleRepository
                .CreateNamespaceFullAccessRole(namespaceName);

            await _roleBindingRepository.BindNamespaceRoleToGroup(
                namespaceName: namespaceName,
                role: namespaceRoleName,
                group: capabilityRegisteredEvent.CapabilityName
            );
        }
    }
}