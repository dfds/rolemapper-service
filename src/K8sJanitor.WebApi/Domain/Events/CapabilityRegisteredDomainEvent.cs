using System;
using Newtonsoft.Json.Linq;
using Serilog;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class CapabilityRegisteredDomainEvent : IDomainEvent<CapabilityRegisteredDomainEventData>
    {
        public string Version { get; }
        public string EventName { get; }
        public Guid XCorrelationId { get; }
        public string XSender { get; }
        public CapabilityRegisteredDomainEventData Payload { get; }

        public CapabilityRegisteredDomainEvent(GeneralDomainEvent domainEvent)
        {
            Version = domainEvent.Version;
            EventName = domainEvent.EventName;
            XCorrelationId = domainEvent.XCorrelationId;
            XSender = domainEvent.XSender;
            Payload = (domainEvent.Payload as JObject)?.ToObject<CapabilityRegisteredDomainEventData>();
            Log.Warning("Instantiating deprecated {Type} with data {Data} ", this.GetType().FullName, Payload);
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