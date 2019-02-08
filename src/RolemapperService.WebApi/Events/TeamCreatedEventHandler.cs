using System.Threading.Tasks;
using RolemapperService.WebApi.Models.ExternalEvents;
using RolemapperService.WebApi.Repositories.Kubernetes;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi
{
    public class TeamCreatedEventHandler : IEventHandler<TeamCreatedEvent>
    {
        private readonly IRoleService _roleService;
        private readonly INamespaceRespoitory _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;

        public TeamCreatedEventHandler(
            IRoleService roleService,
            INamespaceRespoitory namespaceRepository,
            IRoleRepository roleRepository,
            IRoleBindingRepository roleBindingRepository
        )
        {
            _roleService = roleService;
            _namespaceRepository = namespaceRepository;
            _roleRepository = roleRepository;
            _roleBindingRepository = roleBindingRepository;
        }

        public async Task HandleAsync(TeamCreatedEvent teamCreatedEvent)
        {
            var configmapRoleName = teamCreatedEvent.TeamName;
            await _roleService.AddRole(
                roleName: configmapRoleName,
                roleArn: teamCreatedEvent.RoleArn
            );

            var namespaceName = teamCreatedEvent.TeamName;

            await _namespaceRepository.CreateNamespace(
                namespaceName: namespaceName,
                roleName: teamCreatedEvent.RoleName
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