using System.Threading.Tasks;

namespace RolemapperService.WebApi.Services
{
    public interface IRoleService
    {
        Task AddRole(
            string roleName,
            string roleArn
        );
    }
}