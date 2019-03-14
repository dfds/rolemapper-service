using System.Threading.Tasks;

namespace K8sJanitor.WebApi
{
    public interface IEventHandler<T>
    {
        Task HandleAsync(T @event);
    }
}