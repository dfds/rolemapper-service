using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using k8s;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Prometheus;
using RolemapperService.WebApi.Models.ExternalEvents;
using RolemapperService.WebApi.Repositories;
using RolemapperService.WebApi.Repositories.Kubernetes;
using RolemapperService.WebApi.Services;
using RolemapperService.WebApi.Validators;
using RolemapperService.WebApi.Wrappers;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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

            
            if (
                string.IsNullOrWhiteSpace(Configuration["KUBERNETES_SERVICE_HOST"]) == false &&
                string.IsNullOrWhiteSpace(Configuration["KUBERNETES_SERVICE_PORT"]) == false
            )
            {
                services.AddTransient<IKubernetes>(serviceProvider => new Kubernetes(KubernetesClientConfiguration.InClusterConfig()));
            }
            else
            {
                services.AddTransient<IKubernetes>(serviceProvider => new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile()));
            }
    
            
            if (
                string.IsNullOrWhiteSpace(Configuration["S3_AWS_ACCESS_KEY_ID"]) == false &&
                string.IsNullOrWhiteSpace(Configuration["S3_AWS_SECRET_ACCESS_KEY"]) == false &&
                string.IsNullOrWhiteSpace(Configuration["S3_AWS_REGION"]) == false &&
                string.IsNullOrWhiteSpace(Configuration["AWS_S3_BUCKET_NAME_CONFIG_MAP"]) == false
            )
            {
                services.AddTransient<AWSCredentials>(serviceProvider => new BasicAWSCredentials(
                    accessKey: Configuration["S3_AWS_ACCESS_KEY_ID"],
                    secretKey: Configuration["S3_AWS_SECRET_ACCESS_KEY"]
                ));

                services.AddTransient(serviceProvider =>
                    RegionEndpoint.GetBySystemName(Configuration["S3_AWS_REGION"]));

                services.AddTransient<IAmazonS3>(serviceProvider => new AmazonS3Client(
                    credentials: serviceProvider.GetRequiredService<AWSCredentials>(),
                    region: serviceProvider.GetRequiredService<RegionEndpoint>()
                ));

                services.AddTransient<ITransferUtility>(serviceProvider => new TransferUtility(
                    s3Client: serviceProvider.GetRequiredService<IAmazonS3>()
                ));

                services.AddTransient<IPersistenceRepository>(serviceProvider => new AwsS3PersistenceRepository(
                    transferUtility: serviceProvider.GetRequiredService<ITransferUtility>(),
                    bucketName: Configuration["AWS_S3_BUCKET_NAME_CONFIG_MAP"],
                    configMapFileName: Configuration["CONFIG_MAP_FILE_NAME"] ?? "configmap_lovelace_blaster.yml"
                ));
            }
            else
            {
                services.AddTransient<IPersistenceRepository, PersistenceRepositoryStub>();
            }


            services.AddTransient<IConfigMapService, ConfigMapService>();
            services.AddTransient<IAwsAuthConfigMapRepository, AwsAuthConfigMapRepository>();
            services.AddTransient<IAddRoleRequestValidator, AddRoleRequestValidator>();
            services.AddTransient<IAddNamespaceRequestValidator, AddNamespaceRequestValidator>();
            services.AddTransient<INamespaceRespoitory, NamespaceRespoitory>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRoleBindingRepository, RoleBindingRepository>();
            services.AddTransient<IKubernetesWrapper, KubernetesWrapper>();

            // Event handlers
            services.AddTransient<IEventHandler<CapabilityRegisteredEvent>, CapabilityRegisteredEventHandler>();

            services.AddHostedService<MetricHostedService>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
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

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                ResponseWriter = MyPrometheusStuff.WriteResponseAsync
            });

            app.UseMvc();
        }
    }

    public class MetricHostedService : IHostedService
    {
        private const string Host = "0.0.0.0";
        private const int Port = 8080;

        private IMetricServer _metricServer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Staring metric server on {Host}:{Port}");

            _metricServer = new KestrelMetricServer(Host, Port).Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using (_metricServer)
            {
                Console.WriteLine("Shutting down metric server");
                await _metricServer.StopAsync();
                Console.WriteLine("Done shutting down metric server");
            }
        }
    }

    public static class MyPrometheusStuff
    {
        private const string HealthCheckLabelServiceName = "service";
        private const string HealthCheckLabelStatusName = "status";

        private static readonly Gauge HealthChecksDuration;
        private static readonly Gauge HealthChecksResult;

        static MyPrometheusStuff()
        {
            HealthChecksResult = Metrics.CreateGauge("healthcheck",
                "Shows health check status (status=unhealthy|degraded|healthy) 1 for triggered, otherwise 0", new GaugeConfiguration
                {
                    LabelNames = new[] { HealthCheckLabelServiceName, HealthCheckLabelStatusName },
                    SuppressInitialValue = false
                });

            HealthChecksDuration = Metrics.CreateGauge("healthcheck_duration_seconds",
                "Shows duration of the health check execution in seconds",
                new GaugeConfiguration
                {
                    LabelNames = new[] { HealthCheckLabelServiceName },
                    SuppressInitialValue = false
                });
        }

        public static Task WriteResponseAsync(HttpContext httpContext, HealthReport healthReport)
        {
            UpdateMetrics(healthReport);

            httpContext.Response.ContentType = "text/plain";
            return httpContext.Response.WriteAsync(healthReport.Status.ToString());
        }

        private static void UpdateMetrics(HealthReport report)
        {
            foreach (var (key, value) in report.Entries)
            {
                HealthChecksResult.Labels(key, "healthy").Set(value.Status == HealthStatus.Healthy ? 1 : 0);
                HealthChecksResult.Labels(key, "unhealthy").Set(value.Status == HealthStatus.Unhealthy ? 1 : 0);
                HealthChecksResult.Labels(key, "degraded").Set(value.Status == HealthStatus.Degraded ? 1 : 0);

                HealthChecksDuration.Labels(key).Set(value.Duration.TotalSeconds);
            }
        }
    }
}