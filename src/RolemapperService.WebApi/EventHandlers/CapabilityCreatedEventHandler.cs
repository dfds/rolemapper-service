using System;
using System.Threading.Tasks;
using RolemapperService.WebApi.Domain.Events;
using RolemapperService.WebApi.Repositories.Kubernetes;
using RolemapperService.WebApi.Services;

namespace RolemapperService.WebApi.EventHandlers
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