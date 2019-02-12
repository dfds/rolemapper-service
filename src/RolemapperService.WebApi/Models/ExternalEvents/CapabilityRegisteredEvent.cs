using System;

namespace RolemapperService.WebApi.Models.ExternalEvents
{
    public class CapabilityRegisteredEvent
    {
        public CapabilityRegisteredEvent(string capabilityName, string roleArn)
        {
            CapabilityName = capabilityName.ToLower() ?? throw new ArgumentException($"{nameof(capabilityName)} can not be null");
            RoleArn = roleArn ?? throw new ArgumentException($"{nameof(roleArn)} can not be null");
        }

        public string CapabilityName { get; }
        public string RoleArn { get; }
    }
}