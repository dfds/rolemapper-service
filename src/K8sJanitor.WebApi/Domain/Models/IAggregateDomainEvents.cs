using System.Collections.Generic;
using K8sJanitor.WebApi.Domain.Events;

namespace K8sJanitor.WebApi.Domain.Models
{
    public interface IAggregateDomainEvents
    {
        IEnumerable<IDomainEvent<object>> DomainEvents { get; }
        void ClearDomainEvents();
        string GetAggregateId();
    }
}