using System;

namespace K8sJanitor.WebApi.Models.ExternalEvents
{
    public class CapabilityRegisteredEvent
    {
        public CapabilityRegisteredEvent(string capabilityName, string roleArn)
        {
            CapabilityName = capabilityName ?? throw new ArgumentException($"{nameof(capabilityName)} can not be null");
            RoleArn = roleArn ?? throw new ArgumentException($"{nameof(roleArn)} can not be null");
        }

        public string CapabilityName { get; }
        public string RoleArn { get; }
    }
}