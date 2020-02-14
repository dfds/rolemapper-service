using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s.Models;
using K8sJanitor.WebApi.Infrastructure.AWS;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Wrappers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Rest;

namespace K8sJanitor.WebApi.Repositories.Kubernetes
{
    public class NamespaceRepository : INamespaceRepository
    {
        private readonly IKubernetesWrapper _client;

        public NamespaceRepository(IKubernetesWrapper client)
        {
            _client = client;
        }

        
        public async Task CreateNamespaceAsync(NamespaceName namespaceName)
        {
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = namespaceName
                }
            };

            await CreateNamespaceAsync(ns);
        }

        
        public async Task CreateNamespaceAsync(
            NamespaceName namespaceName, 
            IEnumerable<Label> labels
        )
        {
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = namespaceName,
                    Labels = labels.ToDictionary(l => l.Key, l => l.Value),
                }
            };

            await CreateNamespaceAsync(ns);
        }
        
        public async Task AddAnnotations(NamespaceName namespaceName, Dictionary<string, string> annotations)
        {
            var @namespace = await _client.ReadNamespaceAsync(namespaceName);

            var annotationsToStore = @namespace.Metadata.Annotations ?? new Dictionary<string, string>();
            
            annotations.ToList().ForEach(a =>
            {
                var annotationAdded =
                annotationsToStore.TryAdd(a.Key, a.Value);

                if (annotationAdded) {return;}
                
                var annotationToStoreValue = annotationsToStore[a.Key];
                if (annotationToStoreValue != a.Value)
                {
                    // TODO Change the exception type to AnnotationAlreadyExistWithDifferentValue
                    throw new Exception($"The annotation \"{a.Key}\" already exist with the value \"{annotationToStoreValue}\" you are trying to add the value \"{a.Value}\"");
                }
            });
            
            var metadata = new V1ObjectMeta
            {
                Name = namespaceName,
                Annotations = annotationsToStore,
                Labels = @namespace.Metadata.Labels
            };
            var patch = new JsonPatchDocument<V1Namespace>();
            patch.Replace(n => n.Metadata, metadata);
            await _client.PatchNamespaceWithHttpMessagesAsync(new V1Patch(patch), namespaceName);
        }
        
        public async Task CreateNamespaceAsync(string namespaceName, string accountId)
        {
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = namespaceName,
                    Annotations = new Dictionary<string,string>{{"iam.amazonaws.com/permitted", IAM.ConstructRoleArn(accountId, ".*")}}
                }
            };

            await CreateNamespaceAsync(ns);
        }

        private async Task CreateNamespaceAsync(V1Namespace @namespace)
        {
            try
            {
                await _client.CreateNamespaceAsync(@namespace);
            }
            catch (HttpOperationException e) when (e.Response.Content.Contains("\"reason\":\"AlreadyExists\""))
            {
                throw new NamespaceAlreadyExistException(
                    $"Namespace: \"{@namespace.Metadata.Name}\" can not be created, it already exits");
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

        public async Task<IEnumerable<V1Namespace>> GetAllCapabilityNamespacesAsync()
        {
            return await _client.GetAllCapabilityNamespacesAsync();
            
        }
    }
}