using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Moq;
using RolemapperService.WebApi.Repositories.Kubernetes;
using RolemapperService.WebApi.Tests.TestDoubles;
using RolemapperService.WebApi.Wrappers;
using Xunit;

namespace RolemapperService.WebApi.Tests.Repositories.Kubernetes
{
    public class RoleRepositoryFacts
    {

        [Fact]
        public async Task CreateNamespaceFullAccessRole_IncludesCorrectPolicies()
        {
            var k8s = Dummy.Of<IKubernetesWrapper>();
            var sut = new RoleRepository(k8s);

              Mock.Get(k8s).Setup(k => k.CreateNamespacedRoleAsync(It.IsAny<V1Role>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(()=> Task.FromResult(new V1Role()));

            var @namespace = "fancyNamespace";

            var result = await sut.CreateNamespaceFullAccessRole(@namespace);
           

            Moq.Mock.Get(k8s).Verify(s => s.CreateNamespacedRoleAsync(It.Is<V1Role>(r=>
                r.Metadata.NamespaceProperty == @namespace &&
                r.Rules.Count == 4 &&
                r.Rules.Count(rule=>rule.Resources.Any(res=>res.Contains("namespace")))==1), It.Is<string>(n=>n==@namespace),It.IsAny<string>(), It.IsAny<CancellationToken>()));

        }
        
    }
}