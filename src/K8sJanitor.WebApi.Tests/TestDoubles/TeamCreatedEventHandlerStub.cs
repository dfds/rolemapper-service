using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class TeamCreatedEventHandlerStub : IEventHandler<CapabilityRegisteredDomainEvent>
    {
        public bool HandleAsyncGotCalled { get; private set; }

        public Task HandleAsync(CapabilityRegisteredDomainEvent domainEvent)
        {
            HandleAsyncGotCalled = true;
            
            return Task.CompletedTask;
        }
    }
}