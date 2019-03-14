using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;

namespace K8sJanitor.WebApi.EventHandlers
{
    public class CapabilityCreatedEventHandler : IEventHandler<CapabilityCreatedDomainEvent>, WebApi.IEventHandler<CapabilityCreatedDomainEvent>
    {
        
        public Task HandleAsync(CapabilityCreatedDomainEvent domainEvent)
        {
            Console.WriteLine($"Event received: CapabilityCreated with name: {domainEvent.Data.CapabilityName}");

           
            return Task.CompletedTask;
        }
    }
}