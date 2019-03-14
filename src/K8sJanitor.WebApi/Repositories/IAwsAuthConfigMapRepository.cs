using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace K8sJanitor.WebApi.Repositories
{
    public interface IAwsAuthConfigMapRepository
    {
        Task<V1ConfigMap> GetConfigMap();
        Task WriteConfigMap(V1ConfigMap configMapRoleMap);
    }
}