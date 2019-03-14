using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public interface IEventDispatcher
    {
        Task Send(string generalDomainEventJson);
        Task SendAsync(GeneralDomainEvent generalDomainEvent);
    }
    
}