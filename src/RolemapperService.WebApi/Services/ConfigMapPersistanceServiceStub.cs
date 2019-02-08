using System.Threading.Tasks;

namespace RolemapperService.WebApi.Services
{
    public class ConfigMapPersistanceServiceStub :IConfigMapPersistanceService
    {
        public Task StoreConfigMap(string configMapYaml)
        {
            return Task.CompletedTask;
        }
    }
}