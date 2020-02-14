using System;

namespace K8sJanitor.WebApi.Models
{
    public class Namespace
    {
        public string Name { get; set; }
        public Guid CapabilityId { get; set; }
    }
}