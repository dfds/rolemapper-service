using System.Threading.Tasks;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class K8sNamespaceCreatedAndAwsArnConnectedEventHandler : IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>
    {
        private readonly IConfigMapService _configMapService;
        private readonly INamespaceRepository _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;

        public K8sNamespaceCreatedAndAwsArnConnectedEventHandler(
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
        
        public async Task HandleAsync(K8sNamespaceCreatedAndAwsArnConnectedEvent domainEvent)
        {
            return;
            // If event "k8s_namespace_created_and_aws_arn_connected" needs to be handled it can be done here.
        }
    }
}