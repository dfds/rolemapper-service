using System.Collections.Generic;
using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Services
{
    public interface IConfigMapService
    {
        Task AddRole(
            string roleName,
            string roleArn
        );
    }
}