using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Repositories
{
    public interface IConfigMapRoleMapWriteStore
    {
        Task StoreConfigMap(string configMapRoleMap);
    }
}