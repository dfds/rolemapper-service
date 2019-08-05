using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;
using Microsoft.Extensions.Logging;

namespace K8sJanitor.WebApi
{
    public class CapabilityRegisteredEventHandler : IEventHandler<CapabilityRegisteredDomainEvent>
    {
        private readonly IConfigMapService _configMapService;
        private readonly INamespaceRepository _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;
        private readonly ILogger<CapabilityRegisteredEventHandler> _logger;

        public CapabilityRegisteredEventHandler(
            IConfigMapService configMapService,
            INamespaceRepository namespaceRepository,
            IRoleRepository roleRepository,
            IRoleBindingRepository roleBindingRepository,
            ILogger<CapabilityRegisteredEventHandler> logger
        )
        {
            _namespaceRepository = namespaceRepository;
            _roleRepository = roleRepository;
            _roleBindingRepository = roleBindingRepository;
            _logger = logger;
            _configMapService = configMapService;
        }

        public async Task HandleAsync(CapabilityRegisteredDomainEvent capabilityRegisteredDomainEvent)
        {
            _logger.LogWarning($"Handling deprecated CapabilityRegisteredDomainEvent for capability {capabilityRegisteredDomainEvent.Payload.CapabilityName}");    
            
            var capabilityName = capabilityRegisteredDomainEvent.Payload.CapabilityName.ToLower();
            
            var configmapRoleName = capabilityName;
            await _configMapService.AddRole(
                roleName: configmapRoleName,
                roleArn: capabilityRegisteredDomainEvent.Payload.RoleArn
            );

            var namespaceName = capabilityName;

            _logger.LogWarning($"Creating namespace with default role permisison (AccountId: 000000000000)");
            await _namespaceRepository.CreateNamespaceAsync(
                namespaceName: namespaceName,
                accountId: "000000000000"
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