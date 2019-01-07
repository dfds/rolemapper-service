
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;

namespace RolemapperService.WebApi.Services
{
    public class ConfigMapPersistanceService : IConfigMapPersistanceService
    {
        private static readonly string ConfigMapFilename = "ConfigMap.yml";
        private readonly IPersistanceRepository _persistanceRepository;
        
        public ConfigMapPersistanceService(IPersistanceRepository persistanceRepository)
        {
            _persistanceRepository = persistanceRepository;
        }
        public async Task StoreConfigMap(string configMapYaml)
        {
            await _persistanceRepository.StoreFile(ConfigMapFilename, configMapYaml);
        }
    }
}