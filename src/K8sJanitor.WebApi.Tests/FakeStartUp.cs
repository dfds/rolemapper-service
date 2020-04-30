using K8sJanitor.WebApi.Infrastructure.Messaging;
using K8sJanitor.WebApi.Tests.TestDoubles;
using K8sJanitor.WebApi.Wrappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K8sJanitor.WebApi.Tests
{
    public class FakeStartup : Startup
    {
        public FakeStartup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) : base(configuration, hostingEnvironment)
        {

        }

        protected override void AddEventRegistry(IServiceCollection services)
        {
            services.AddSingleton<IDomainEventRegistry, DomainEventRegistry>();
        }

        protected override void AddMetricServices(IServiceCollection services)
        {
            //Do nothing to avoid loading metric server.
        }

        protected override void AddK8sServices(IServiceCollection services)
        {
            services.AddTransient<IKubernetesWrapper>(k => new KubernetesWrapperDummy());
        }
    }
}
