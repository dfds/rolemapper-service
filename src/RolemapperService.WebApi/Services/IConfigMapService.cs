using System.Collections.Generic;
using System.Threading.Tasks;

namespace RolemapperService.WebApi.Services
{
    public interface IConfigMapService
    {
        Task<string> AddReadOnlyRoleMapping(string roleName, string roleArn);
        Task<string> AddRoleMapping(string roleArn, string userName, IList<string> groups);
    }
}