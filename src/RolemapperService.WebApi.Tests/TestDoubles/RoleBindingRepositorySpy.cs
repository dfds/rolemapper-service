using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories.Kubernetes;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class RoleBindingRepositorySpy : IRoleBindingRepository
    {
        public List<Tuple<string, string, string>> NamespaceRoleToGroupBindings { get; }

        public RoleBindingRepositorySpy()
        {
            NamespaceRoleToGroupBindings = new List<Tuple<string, string, string>>();
        }
        public Task BindNamespaceRoleToGroup(string namespaceName, string role, string @group)
        {
            NamespaceRoleToGroupBindings.Add(new Tuple<string, string, string>(namespaceName, role, @group));
            
            return Task.CompletedTask;
        }
    }
}