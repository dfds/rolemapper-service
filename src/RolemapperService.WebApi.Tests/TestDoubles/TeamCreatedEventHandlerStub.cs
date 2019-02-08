using System.Threading.Tasks;
using RolemapperService.WebApi.Models.ExternalEvents;

namespace RolemapperService.WebApi.Tests.TestDoubles
{
    public class TeamCreatedEventHandlerStub : IEventHandler<CapabilityRegisteredEvent>
    {
        public bool HandleAsyncGotCalled { get; private set; }

        public Task HandleAsync(CapabilityRegisteredEvent @event)
        {
            HandleAsyncGotCalled = true;
            
            return Task.CompletedTask;
        }
    }
}