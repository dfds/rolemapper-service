using System;

namespace K8sJanitor.WebApi.Infrastructure.Api.Models
{
    public class Namespace
    {
        public string Name { get; set; }
        public Guid CapabilityId { get; set; }
    }
}