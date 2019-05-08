using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Wrappers;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests.Repositories.Kubernetes.RoleBindingRepository
{
    public class BindNamespaceRoleToGroupFacts
    {
        [Fact]
        public async Task Bind_To_A_Existing_Namespace()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

            var client = new k8s.Kubernetes(config);
            var wrapper = new KubernetesWrapper(client);
            var namespaceRepository = new NamespaceRepository(client);
            var roleRepository = new WebApi.Repositories.Kubernetes.RoleRepository(wrapper);
            var sut = new WebApi.Repositories.Kubernetes.RoleBindingRepository(client);
    
            var subjectNameSpace = "namespace-with-role-test";
            var awsRoleName = "notUSed";
            var groupName = "a-test-group";

            try
            {
                await namespaceRepository.CreateNamespaceAsync(subjectNameSpace, awsRoleName);
                var roleName = await roleRepository.CreateNamespaceFullAccessRole(subjectNameSpace);
                
                // Act
                await sut.BindNamespaceRoleToGroup(
                    subjectNameSpace, 
                    roleName, 
                    groupName
                );
                
                // Assert
                var roleBindings = await client.ListNamespacedRoleBindingAsync(subjectNameSpace);
                roleBindings.Items.Single();
            }
            finally
            {
                client.DeleteNamespace(
                    body: new V1DeleteOptions(),
                    name: subjectNameSpace
                );
            }
        }
    }
}