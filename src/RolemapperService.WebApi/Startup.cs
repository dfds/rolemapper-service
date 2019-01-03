using k8s;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RolemapperService.WebApi.Repositories;
using RolemapperService.WebApi.Services;
using RolemapperService.WebApi.Validators;

namespace RolemapperService.WebApi
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<IKubernetes>(serviceProvider =>
                        {
                            var config =
                            _hostingEnvironment.IsDevelopment()
                                ? KubernetesClientConfiguration.BuildConfigFromConfigFile()
                                : KubernetesClientConfiguration.InClusterConfig();

                            return new Kubernetes(config);
                        });

            services.AddTransient<IConfigMapService, ConfigMapService>();
            services.AddTransient<IKubernetesService, KubernetesService>();
            services.AddTransient<IKubernetesRepository, KubernetesRepository>();
            services.AddTransient<IAddRoleRequestValidator, AddRoleRequestValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();
        }
    }
}
