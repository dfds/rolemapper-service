
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
        private readonly string _configMapFileName;
        private readonly IPersistanceRepository _persistanceRepository;
        
        public ConfigMapPersistanceService(
            IPersistanceRepository persistanceRepository, 
            string configMapFileName = "configmap_lovelace_blaster.yml")
        {
            _persistanceRepository = persistanceRepository;
            _configMapFileName = configMapFileName;
        }
        public async Task StoreConfigMap(string configMapYaml)
        {
            await _persistanceRepository.StoreFile(_configMapFileName, configMapYaml);
        }
    }
}