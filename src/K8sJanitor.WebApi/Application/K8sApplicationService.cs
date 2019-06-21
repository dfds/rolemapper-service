using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Application
{
    public class K8sApplicationService : IK8sApplicationService
    {

        public K8sApplicationService()
        {
            
        }
        
        public Task TestCreated(string description)
        {
            throw new System.NotImplementedException();
        }
    }
}