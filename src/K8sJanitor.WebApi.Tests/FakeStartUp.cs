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

        protected override void AddMetricServices(IServiceCollection services)
        {
            //Do nothing to avoid loading metric server.
        }
    }
}
