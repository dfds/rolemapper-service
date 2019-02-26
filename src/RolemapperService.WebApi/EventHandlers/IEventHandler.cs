using System;
using System.Threading.Tasks;

namespace RolemapperService.WebApi.EventHandlers
{
    public interface IEventHandler<in T>
    {
        Task HandleAsync(T domainEvent);
    }
}