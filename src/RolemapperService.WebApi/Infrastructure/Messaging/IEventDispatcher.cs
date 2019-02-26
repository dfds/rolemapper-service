using System.Threading.Tasks;
using RolemapperService.WebApi.Domain.Events;

namespace RolemapperService.WebApi.Infrastructure.Messaging
{
    public interface IEventDispatcher
    {
        Task Send(string generalDomainEventJson);
        Task SendAsync(GeneralDomainEvent generalDomainEvent);
    }
    
}