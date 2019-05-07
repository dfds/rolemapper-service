using System;
using Newtonsoft.Json.Linq;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class CapabilityCreatedDomainEvent : IDomainEvent<CapabilityCreatedData>
    {
        public Guid MessageId { get; private set; }

        public string Type { get; private set; }

        public CapabilityCreatedData Data { get; private set; }

        public CapabilityCreatedDomainEvent(GeneralDomainEvent domainEvent)
        {
            MessageId = domainEvent.MessageId;
            Type = domainEvent.Type;
            Data = (domainEvent.Data as JObject)?.ToObject<CapabilityCreatedData>();
        }
    }

    public class CapabilityCreatedData
    {
        public CapabilityCreatedData(
            Guid capabilityId, 
            string capabilityName,
            Guid contextId, 
            string contextName
        )
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
            ContextId = contextId;
            ContextName = contextName;
        }
        public Guid CapabilityId { get; }
        
        public string CapabilityName { get; }
        public Guid ContextId { get; }
        public string ContextName { get; }
    }
}