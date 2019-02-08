using System.Threading.Tasks;
using RolemapperService.WebApi.Models.ExternalEvents;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class TeamCreatedEventHandlerStub : IEventHandler<TeamCreatedEvent>
    {
        public bool HandleAsyncGotCalled { get; private set; }

        public Task HandleAsync(TeamCreatedEvent @event)
        {
            HandleAsyncGotCalled = true;
            
            return Task.CompletedTask;
        }
    }
}