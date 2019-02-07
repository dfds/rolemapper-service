using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using RolemapperService.WebApi.Repositories.Kubernetes;
using Xunit;

namespace RolemapperService.WebApi.IntegrationTests.Repositories.Kubernetes.RoleBindingRepository
{
    public class BindNamespaceRoleToGroupFacts
    {
        [Fact]
        public async Task Bind_To_A_Existing_Namespace()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

            var client = new k8s.Kubernetes(config);

            var namespaceRepository = new NamespaceRespoitory(client);
            var roleRepository = new WebApi.Repositories.Kubernetes.RoleRepository(client);
            var sut = new WebApi.Repositories.Kubernetes.RoleBindingRepository(client);
    
            var subjectNameSpace = "namespace-with-role-test";
            var awsRoleName = "notUSed";
            var groupName = "a-test-group";

            try
            {
                await namespaceRepository.CreateNamespace(subjectNameSpace, awsRoleName);
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