using K8sJanitor.WebApi.Domain.Events;
using System.Threading.Tasks;

namespace K8sJanitor.WebApi.EventHandlers
{
    public interface IEventHandler
    {
    }

    public interface IEventHandler<in T> : IEventHandler where T : IEvent
    {
        Task HandleAsync(T @event);
    }
}