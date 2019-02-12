using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Rest;

namespace RolemapperService.WebApi.Repositories.Kubernetes
{
    public class NamespaceRespoitory : INamespaceRespoitory
    {
        private readonly IKubernetes _client;

        public NamespaceRespoitory(IKubernetes client)
        {
            _client = client;
        }

        public async Task CreateNamespace(string namespaceName, string roleName)
        {
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = namespaceName,
                    Annotations = new Dictionary<string, string> {{"iam.amazonaws.com/permitted", roleName}}
                }
            };

            try
            {
                await _client.CreateNamespaceAsync(ns);
            }
            catch (HttpOperationException e) when (e.Response.Content.Length != 0)
            {
                throw new Exception(
                    "Error occured while communicating with k8s:" + Environment.NewLine +
                    e.Response.Content
                    , e
                );
            }
        }
    }
}