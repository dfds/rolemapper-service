using System;
using Newtonsoft.Json.Linq;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class ContextAccountCreatedDomainEvent : IDomainEvent<ContextAccountCreatedDomainEventData>
    {
        public Guid MessageId { get; private set; }
        public string Type { get; private set; }
        public ContextAccountCreatedDomainEventData Data { get; private set;}

        public ContextAccountCreatedDomainEvent(GeneralDomainEvent domainEvent)
        {
            MessageId = domainEvent.MessageId;
            Type = domainEvent.Type;
            Data = (domainEvent.Data as JObject)?.ToObject<ContextAccountCreatedDomainEventData>();
        }
    }

    public class ContextAccountCreatedDomainEventData
    {
        public Guid CapabilityId { get; private set; }
        public string CapabilityName { get; private set; }
        public Guid ContextId { get; private set; }
        public string ContextName { get; private set; }
        public string AccountId { get; private set; }
        public string RoleArn { get; private set; }
        public string RoleEmail { get; private set; }

        public ContextAccountCreatedDomainEventData(
            Guid capabilityId, 
            string capabilityName, 
            Guid contextId, 
            string contextName, 
            string accountId, 
            string roleArn, 
            string roleEmail
        )
        {
            CapabilityId = capabilityId;
            CapabilityName = capabilityName;
            ContextId = contextId;
            ContextName = contextName;
            AccountId = accountId;
            RoleArn = roleArn;
            RoleEmail = roleEmail;
        }
    }
}