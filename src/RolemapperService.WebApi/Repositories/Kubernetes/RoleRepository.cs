using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace RolemapperService.WebApi.Repositories.Kubernetes
{
    public class RoleRepository
    {
        private readonly IKubernetes _client;

        public RoleRepository(IKubernetes client)
        {
            _client = client;
        }

        public async Task<string> CreateNamespaceFullAccessRole(string namespaceName)
        {
            var role = new V1Role
            {
                Metadata = new V1ObjectMeta
                {
                    Name = $"{namespaceName}-fullaccess",
                    NamespaceProperty = namespaceName
                },
                Rules = new List<V1PolicyRule>
                {
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "",
                            "extensions",
                            "apps"
                        },
                        Resources = new List<string>
                        {
                            "*"
                        },
                        // ResourceNames = new List<string>
                        // {
                        //     "*"
                        // },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    },
                    new V1PolicyRule
                    {
                        ApiGroups = new List<string>
                        {
                            "batch"
                        },
                        Resources = new List<string>
                        {
                            "jobs",
                            "cronjobs"
                        },
                        // ResourceNames = new List<string>
                        // {
                        //     "*"
                        // },
                        Verbs = new List<string>
                        {
                            "*"
                        }
                    }
                }
            };
            
            var result = await _client.CreateNamespacedRoleAsync(role, namespaceName);

            return result?.Metadata?.Name;
        }
    }
}