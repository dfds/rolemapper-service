using System.Threading;
using k8s.Models;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Tests.TestDoubles;
using K8sJanitor.WebApi.Wrappers;
using Moq;
using Xunit;

namespace K8sJanitor.WebApi.Tests.Repositories.Kubernetes
{
    public class NamespaceRepositoryFacts
    {
        [Fact]
        public async void CreateNamespaceSets()
        {
            var k8s = Dummy.Of<IKubernetesWrapper>();
            var sut = new NamespaceRepository(k8s);
            var nsName = "ababab";
            var namespaceName = NamespaceName.Create(nsName);


            await sut.CreateNamespaceAsync(namespaceName);

            // Assert
            var mock = Mock.Get(k8s);
            mock.Verify(m=>m.CreateNamespaceAsync(It.Is<V1Namespace>(ns=>ns.Metadata.Name.Equals(nsName)),null, CancellationToken.None));
        }
    }
}