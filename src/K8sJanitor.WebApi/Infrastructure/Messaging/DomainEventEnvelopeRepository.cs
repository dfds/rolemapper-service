using System.Collections.Generic;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Infrastructure.Persistence;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class DomainEventEnvelopeRepository
    {
        private readonly K8sServiceDbContext _dbContext;

        public DomainEventEnvelopeRepository(K8sServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(IEnumerable<DomainEventEnvelope> domainEvents)
        {
            await _dbContext.DomainEvents.AddRangeAsync(domainEvents);
        }
    }
}