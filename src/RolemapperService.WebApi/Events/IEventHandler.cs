using System.Threading.Tasks;

namespace RolemapperService.WebApi
{
    public interface IEventHandler<T>
    {
        Task HandleAsync(T @event);
    }
}