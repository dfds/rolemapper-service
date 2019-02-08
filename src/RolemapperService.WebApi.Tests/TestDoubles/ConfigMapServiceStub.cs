using System.Threading.Tasks;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class ConfigMapServiceStub : IConfigMapService
    {
        public Task AddRole(string roleName, string roleArn)
        {
            return Task.CompletedTask;
        }
    }
}