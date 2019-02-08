using System.Threading.Tasks;
using RolemapperService.WebApi.Models.ExternalEvents;
using RolemapperService.WebApi.Repositories.Kubernetes;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi
{
    public class TeamCreatedEventHandler : IEventHandler<TeamCreatedEvent>
    {
        private readonly IConfigMapService _configMapService;
        private readonly INamespaceRespoitory _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;

        public TeamCreatedEventHandler(
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

        public async Task HandleAsync(TeamCreatedEvent teamCreatedEvent)
        {
            var configmapRoleName = teamCreatedEvent.TeamName;
            await _configMapService.AddRole(
                roleName: configmapRoleName,
                roleArn: teamCreatedEvent.RoleArn
            );

            var namespaceName = teamCreatedEvent.TeamName;

            await _namespaceRepository.CreateNamespace(
                namespaceName: namespaceName,
                roleName: configmapRoleName
            );

            var namespaceRoleName = await _roleRepository
                .CreateNamespaceFullAccessRole(namespaceName);

            await _roleBindingRepository.BindNamespaceRoleToGroup(
                namespaceName: namespaceName,
                role: namespaceRoleName,
                group: teamCreatedEvent.TeamName
            );
        }
    }
}