using System.Threading.Tasks;

namespace RolemapperService.WebApi.Repositories
{
    public class PersistenceRepositoryStub :IPersistenceRepository
    {
        public Task StoreFile(string content)
        {
            return Task.CompletedTask;
        }
    }
}