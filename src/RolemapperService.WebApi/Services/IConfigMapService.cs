using System.Collections.Generic;
using System.Threading.Tasks;

namespace RolemapperService.WebApi.Services
{
    public interface IConfigMapService
    {
        Task AddRole(
            string roleName,
            string roleArn
        );
    }
}