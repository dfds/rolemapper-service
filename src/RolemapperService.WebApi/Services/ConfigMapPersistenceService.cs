
using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;

namespace RolemapperService.WebApi.Services
{
    public class ConfigMapPersistenceService : IConfigMapPersistanceService
    {
        private readonly string _configMapFileName;
        private readonly IPersistanceRepository _persistenceRepository;
        
        public ConfigMapPersistenceService(
            IPersistanceRepository persistenceRepository, 
            string configMapFileName = "configmap_lovelace_blaster.yml")
        {
            _persistenceRepository = persistenceRepository;
            _configMapFileName = configMapFileName;
        }
        
        public async Task StoreConfigMap(string configMapYaml)
        {
            await _persistenceRepository.StoreFile(_configMapFileName, configMapYaml);
        }
    }
}