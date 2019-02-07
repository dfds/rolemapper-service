using System.Threading.Tasks;

namespace RolemapperService.WebApi.Services
{
    public interface IRoleService
    {
        Task CreateRole(
            string roleName,
            string roleArn
        );
    }
}