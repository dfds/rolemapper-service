using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sJanitor.WebApi.Wrappers;
using Microsoft.Rest;
using Serilog;

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
            var roleBindingName = $"{role}-to-{@group}";
            var roleBinding = new V1RoleBinding
            {
                Metadata = new V1ObjectMeta
                {
                    Name = roleBindingName,
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

            try
            {
                await _client.CreateNamespacedRoleBindingAsync(roleBinding, namespaceName);
            }
            catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.Conflict)
            {
                Log.Warning($"RoleBinding {roleBindingName} already exist for namespace {namespaceName}");
                throw new RoleBindingAlreadyExistInNamespaceException($"RoleBinding {roleBindingName} already exist for namespace {namespaceName}", roleBindingName, namespaceName);
            }
        }
    }
}