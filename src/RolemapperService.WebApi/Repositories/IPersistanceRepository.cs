using System.Threading.Tasks;

namespace RolemapperService.WebApi.Repositories
{
    public interface IPersistanceRepository
    {
        Task StoreFile(string fileName, string content);
    }
}