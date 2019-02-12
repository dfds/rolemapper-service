using System.Threading.Tasks;

namespace RolemapperService.WebApi.Repositories
{
    public interface IPersistenceRepository
    {
        Task StoreFile(string content);
    }
}