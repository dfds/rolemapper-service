using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class NamespaceRepositorySpy : INamespaceRepository
    {
        public List<Namespace> Namespaces { get; }

        public NamespaceRepositorySpy()
        {
            Namespaces = new List<Namespace>();
        }

        public Task CreateNamespaceAsync(string namespaceName, string accountId)
        {
            var @namespace = new Namespace
            {
                NamespaceName = NamespaceName.Create(namespaceName),
                AccountId = accountId
            };

            Namespaces.Add(@namespace);

            return Task.CompletedTask;
        }

        public Task CreateNamespaceAsync(NamespaceName namespaceName, IEnumerable<Label> labels)
        {
            var @namespace = new Namespace
            {
                NamespaceName = namespaceName,
                Labels = labels
            };

            Namespaces.Add(@namespace);

            return Task.CompletedTask;
        }

        public Task CreateNamespaceAsync(NamespaceName namespaceName)
        {
            Namespaces.Add(new Namespace {NamespaceName = namespaceName});

            return Task.CompletedTask;
        }

        public Task AddAnnotations(NamespaceName namespaceName, Dictionary<string, string> annotations)
        {
            var @namespace = Namespaces.Single(n => n.NamespaceName.Equals(namespaceName));

            if (@namespace.Annotations.IsNullOrEmpty())
            {
                @namespace.Annotations = annotations;

                return Task.CompletedTask;
            }

            var dictionaries = new[] {annotations, @namespace.Annotations};

            var resultAnnotations = dictionaries.SelectMany(dict => dict)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            @namespace.Annotations = resultAnnotations;
            
            return Task.CompletedTask;
        }

        public class Namespace
        {
            public NamespaceName NamespaceName { get; set; }
            public string AccountId { get; set; }
            public IDictionary<string, string> Annotations { get; set; }
            public IEnumerable<Label> Labels { get; set; }
        }
    }
}