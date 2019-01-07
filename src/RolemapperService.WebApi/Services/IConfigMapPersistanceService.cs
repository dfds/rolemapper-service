using System.Collections.Generic;
using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;

namespace RolemapperService.WebApi.Services
{
    public interface IConfigMapPersistanceService
    {
        Task StoreConfigMap(string configMapYaml);
    }
}