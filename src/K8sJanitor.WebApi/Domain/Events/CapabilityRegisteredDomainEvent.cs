using System;
using Newtonsoft.Json.Linq;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class CapabilityRegisteredDomainEvent : IDomainEvent<CapabilityRegisteredDomainEventData>
    {
        public Guid MessageId { get; }
        public string Type { get; }
        public CapabilityRegisteredDomainEventData Data { get; }

        public CapabilityRegisteredDomainEvent(GeneralDomainEvent domainEvent)
        {
            MessageId = domainEvent.MessageId;
            Type = domainEvent.Type;
            Data = (domainEvent.Data as JObject)?.ToObject<CapabilityRegisteredDomainEventData>();
        }
    }

    public class CapabilityRegisteredDomainEventData
    {
        public CapabilityRegisteredDomainEventData(string capabilityName, string roleArn)
        {
            CapabilityName = capabilityName;
            RoleArn = roleArn;
        }

        public string CapabilityName { get; }
        public string RoleArn { get; }
    }
}