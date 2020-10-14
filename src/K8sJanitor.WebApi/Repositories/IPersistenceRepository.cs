using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Repositories
{
    public interface IPersistenceRepository
    {
        Task StoreFile(string content, string contentType = "application/octet-stream");
    }
}