using System.Collections.Generic;
    using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sJanitor.WebApi.Wrappers;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public class RoleBindingRepository : IRoleBindingRepository
    {
        private readonly IKubernetesWrapper _client;

        public RoleBindingRepository(IKubernetesWrapper client)
        {
            _client = client;
        }

        public async Task BindNamespaceRoleToGroup(string namespaceName, string role, string group)
        {
            var roleBinding = new V1RoleBinding
            {
                Metadata = new V1ObjectMeta
                {
                    Name = $"{role}-to-{group}",
                    NamespaceProperty = namespaceName
                },
                Subjects = new List<V1Subject>{new V1Subject
                {
                    Kind = "Group",
                    Name = group,
                    ApiGroup = "rbac.authorization.k8s.io",
                }},
                RoleRef = new V1RoleRef
                {
                    Kind = "Role",
                    Name = role,
                    ApiGroup = "rbac.authorization.k8s.io"
                }
            };

            await _client.CreateNamespacedRoleBindingAsync(roleBinding, namespaceName);
        }
    }
}