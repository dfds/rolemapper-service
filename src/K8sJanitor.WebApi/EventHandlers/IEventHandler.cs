using System.Threading.Tasks;

namespace K8sJanitor.WebApi.EventHandlers
{
    public interface IEventHandler<in T>
    {
        Task HandleAsync(T domainEvent);
    }
}