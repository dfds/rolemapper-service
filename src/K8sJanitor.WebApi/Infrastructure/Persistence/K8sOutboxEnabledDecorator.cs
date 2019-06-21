using System.Threading.Tasks;
using K8sJanitor.WebApi.Application;
using K8sJanitor.WebApi.Infrastructure.Messaging;

namespace K8sJanitor.WebApi.Infrastructure.Persistence
{
    public class K8sOutboxEnabledDecorator : IK8sApplicationService
    {
        private readonly Outbox _outbox;
        private readonly IK8sApplicationService _inner;
        private readonly K8sServiceDbContext _dbContext;

        public K8sOutboxEnabledDecorator(Outbox outbox, K8sApplicationService inner,
            K8sServiceDbContext dbContext)
        {
            _inner = inner;
            _outbox = outbox;
            _dbContext = dbContext;
        }
        
        // DB Part to be skipped for now. TODO
        public Task TestCreated(string description)
        {
            throw new System.NotImplementedException();
        }
    }

    public class K8sTransactionalDecorator : IK8sApplicationService
    {
        private readonly IK8sApplicationService _inner;
        private readonly K8sServiceDbContext _dbContext;

        public K8sTransactionalDecorator(IK8sApplicationService inner, K8sServiceDbContext dbContext)
        {
            _inner = inner;
            _dbContext = dbContext;
        }
        
        public Task TestCreated(string description)
        {
            throw new System.NotImplementedException();
        }
    }
    
    
}