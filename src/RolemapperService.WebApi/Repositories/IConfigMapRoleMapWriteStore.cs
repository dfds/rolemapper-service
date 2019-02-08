using System.Threading.Tasks;

namespace RolemapperService.WebApi.Repositories
{
    public interface IConfigMapRoleMapWriteStore
    {
        Task StoreConfigMap(string configMapRoleMap);
    }
}