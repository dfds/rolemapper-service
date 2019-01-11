using System.Threading.Tasks;
using RolemapperService.WebApi.Repositories;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class PersistanceRepositoryStub : IPersistanceRepository
    {
        public async Task StoreFile(string fileName, string content)
        {
            await Task.FromResult(0);
        }
    }
}
