using System.Threading.Tasks;
using K8sJanitor.WebApi.Services;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class ConfigMapServiceStub : IConfigMapService
    {
        public Task AddRole(string roleName, string roleArn)
        {
            return Task.CompletedTask;
        }
    }
}