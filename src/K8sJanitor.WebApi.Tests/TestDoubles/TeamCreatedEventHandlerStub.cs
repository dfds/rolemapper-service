using System.Threading.Tasks;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Models.ExternalEvents;

namespace K8sJanitor.WebApi.Tests.TestDoubles
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