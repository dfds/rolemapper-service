using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using k8s;
using K8sJanitor.WebApi.Application;
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Extensions;
using K8sJanitor.WebApi.HealthChecks;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using K8sJanitor.WebApi.Repositories;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;
using K8sJanitor.WebApi.Validators;
using K8sJanitor.WebApi.Wrappers;
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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace K8sJanitor.WebApi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc((options) => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddCheck<S3BucketHealthCheck>("S3 bucket");

            services.AddLazyResolution();

            AddAwsResources(services);
            AddK8sServices(services);
            AddMetricServices(services);
            AddKafkaServices(services);
            AddEventRegistry(services);
            AddEventDispatcher(services);
            AddEventHandlers(services);

            ConfigureDomainServices(services);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

        protected virtual void ConfigureDomainServices(IServiceCollection services)
        {
            //Services
            services.AddTransient<IK8sApplicationService, K8sApplicationService>();
            services.AddTransient<IConfigMapService, ConfigMapService>();

            //Repositories
            services.AddTransient<IAwsAuthConfigMapRepository, AwsAuthConfigMapRepository>();
            services.AddTransient<INamespaceRepository, NamespaceRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRoleBindingRepository, RoleBindingRepository>();

            //Validators
            services.AddTransient<IAddRoleRequestValidator, AddRoleRequestValidator>();
            services.AddTransient<IAddNamespaceRequestValidator, AddNamespaceRequestValidator>();
        }

        protected virtual void AddAwsResources(IServiceCollection services)
        {
            if (string.IsNullOrWhiteSpace(Configuration["AWS_S3_BUCKET_REGION"]) ||
                string.IsNullOrWhiteSpace(Configuration["AWS_S3_BUCKET_NAME_CONFIG_MAP"]) ||
                string.IsNullOrWhiteSpace(Configuration["CONFIG_MAP_FILE_NAME"]))
            {
                services.AddTransient<IPersistenceRepository, PersistenceRepositoryStub>();

                return;
            }

            var regionEndpoint = RegionEndpoint.GetBySystemName(Configuration["AWS_S3_BUCKET_REGION"]);

            services.AddTransient<IAmazonS3>(serviceProvider => new AmazonS3Client(regionEndpoint));

            services.AddTransient<ITransferUtility>(serviceProvider => new TransferUtility(
                s3Client: serviceProvider.GetRequiredService<IAmazonS3>()
            ));

            services.AddTransient<IPersistenceRepository>(serviceProvider => new AwsS3PersistenceRepository(
                transferUtility: serviceProvider.GetRequiredService<ITransferUtility>(),
                bucketName: Configuration["AWS_S3_BUCKET_NAME_CONFIG_MAP"],
                configMapFileName: Configuration["CONFIG_MAP_FILE_NAME"]
            ));

            services.AddTransient<S3BucketHealthCheck>(serviceProvider => new S3BucketHealthCheck(
                amazonS3: serviceProvider.GetRequiredService<IAmazonS3>(),
                bucketName: Configuration["AWS_S3_BUCKET_NAME_CONFIG_MAP"])
            );
        }

        protected virtual void AddEventRegistry(IServiceCollection services)
        {
            services.AddSingleton<IDomainEventRegistry, DomainEventRegistry>();
            services.AddSingleton(provider => {
                return new DomainEventRegistration
                {
                    EventTypeName = "aws_context_account_created",
                    EventType = typeof(ContextAccountCreatedDomainEvent),
                    Topic = "build.selfservice.events.capabilities"
                };
            });
            services.AddSingleton(provider => {
                return new DomainEventRegistration
                {
                    EventTypeName = "capability_registered",
                    EventType = typeof(CapabilityRegisteredDomainEvent),
                    Topic = "build.selfservice.events.capabilities"
                };
            });
            services.AddSingleton(provider => {
                return new DomainEventRegistration
                {
                    EventTypeName = "k8s_namespace_created_and_aws_arn_connected",
                    EventType = typeof(K8sNamespaceCreatedAndAwsArnConnectedEvent),
                    Topic = "build.selfservice.events.capabilities"
                };
            });
        }

        protected virtual void AddEventDispatcher(IServiceCollection services)
        {
            services.AddSingleton<IPublishingEventsQueue, PublishingEventsQueue>();
            services.AddTransient<IEventDispatcher, EventDispatcher>();

            services.AddHostedService<PublishingService>();
        }

        protected virtual void AddEventHandlers(IServiceCollection services)
        {
            services.AddTransient<IEventHandler, ContextAccountCreatedDomainEventHandler>();
            services.AddTransient<IEventHandler<ContextAccountCreatedDomainEvent>, ContextAccountCreatedDomainEventHandler>();
            services.AddTransient<IEventHandler, CapabilityRegisteredEventHandler>();
            services.AddTransient<IEventHandler<CapabilityRegisteredDomainEvent>, CapabilityRegisteredEventHandler>();
            services.AddTransient<IEventHandler, K8sNamespaceCreatedAndAwsArnConnectedEventHandler>();
            services.AddTransient<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>, K8sNamespaceCreatedAndAwsArnConnectedEventHandler>();
        }

        protected virtual void AddK8sServices(IServiceCollection services)
        {
            if (
                string.IsNullOrWhiteSpace(Configuration["KUBERNETES_SERVICE_HOST"]) == false &&
                string.IsNullOrWhiteSpace(Configuration["KUBERNETES_SERVICE_PORT"]) == false
            )
            {
                services.AddTransient<IKubernetesWrapper>(k =>
                    new KubernetesWrapper(new Kubernetes(KubernetesClientConfiguration.InClusterConfig())));
            }
            else if (ExecuteAgainstK8s.Allowed)
            {
                services.AddTransient<IKubernetesWrapper>(k =>
                    new KubernetesWrapper(new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile())));
            }
        }

        protected virtual void AddKafkaServices(IServiceCollection services)
        {
            services.AddTransient<KafkaConsumerFactory.KafkaConfiguration>();
            services.AddTransient<KafkaPublisherFactory>();
            services.AddTransient<KafkaConsumerFactory>();

            services.AddHostedService<KafkaConsumerHostedService>();
        }
                
        protected virtual void AddMetricServices(IServiceCollection services)
        {
            services.AddHostedService<MetricHostedService>();
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
                "Shows health check status (status=unhealthy|degraded|healthy) 1 for triggered, otherwise 0",
                new GaugeConfiguration
                {
                    LabelNames = new[] {HealthCheckLabelServiceName, HealthCheckLabelStatusName},
                    SuppressInitialValue = false
                });

            HealthChecksDuration = Metrics.CreateGauge("healthcheck_duration_seconds",
                "Shows duration of the health check execution in seconds",
                new GaugeConfiguration
                {
                    LabelNames = new[] {HealthCheckLabelServiceName},
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