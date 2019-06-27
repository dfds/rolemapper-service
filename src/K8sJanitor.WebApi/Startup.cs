using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using k8s;
using k8s.Exceptions;
using K8sJanitor.WebApi.Application;
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
using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.HealthChecks;
using K8sJanitor.WebApi.Infrastructure.Messaging;
using K8sJanitor.WebApi.Repositories;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;
using K8sJanitor.WebApi.Validators;
using K8sJanitor.WebApi.Wrappers;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace K8sJanitor.WebApi
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

        public IServiceCollection AddPersistenceRepository(IServiceCollection services)
        {
            if (string.IsNullOrWhiteSpace(Configuration["AWS_S3_BUCKET_REGION"]) ||
                string.IsNullOrWhiteSpace(Configuration["AWS_S3_BUCKET_NAME_CONFIG_MAP"]) ||
                string.IsNullOrWhiteSpace(Configuration["CONFIG_MAP_FILE_NAME"]))
            {
                services.AddTransient<IPersistenceRepository, PersistenceRepositoryStub>();

                return services;
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
            return services;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            if (
                string.IsNullOrWhiteSpace(Configuration["KUBERNETES_SERVICE_HOST"]) == false &&
                string.IsNullOrWhiteSpace(Configuration["KUBERNETES_SERVICE_PORT"]) == false
            )
            {
                services.AddTransient<IKubernetesWrapper>(k =>
                    new KubernetesWrapper(new Kubernetes(KubernetesClientConfiguration.InClusterConfig())));
            }
            else if(ExecuteAgainstK8s.Allowed)
            {  
                services.AddTransient<IKubernetesWrapper>(k =>
                    new KubernetesWrapper(new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile())));
            }
            else
            {
                services.AddTransient<IKubernetesWrapper>(k =>
                    new KubernetesWrapper(null));
            }
            

            services = AddPersistenceRepository(services);

            services.AddTransient<IConfigMapService, ConfigMapService>();
            services.AddTransient<IAwsAuthConfigMapRepository, AwsAuthConfigMapRepository>();
            services.AddTransient<IAddRoleRequestValidator, AddRoleRequestValidator>();
            services.AddTransient<IAddNamespaceRequestValidator, AddNamespaceRequestValidator>();
            services.AddTransient<INamespaceRepository, NamespaceRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRoleBindingRepository, RoleBindingRepository>();

            // Event handlers

            services.AddHostedService<MetricHostedService>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddCheck<S3BucketHealthCheck>("S3 bucket");


            ConfigureDomainEvents(services);

            services.AddHostedService<KafkaConsumerHostedService>();
        }


        private static void ConfigureDomainEvents(IServiceCollection services)
        {
            var eventRegistry = new DomainEventRegistry();
            services.AddSingleton(eventRegistry);
            services.AddTransient<IEventHandler<ContextAccountCreatedDomainEvent>, ContextAccountCreatedDomainEventHandler>();
            services.AddTransient<IEventHandler<CapabilityRegisteredDomainEvent>, CapabilityRegisteredEventHandler>();
            services.AddTransient<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>, K8sNamespaceCreatedAndAwsArnConnectedEventHandler>(); // Determine whether this is necessary, probably not.

            // Event publishing
            services.AddTransient<K8sApplicationService>();

            services.AddTransient<KafkaConsumerFactory.KafkaConfiguration>();
            services.AddTransient<KafkaPublisherFactory>();
            services.AddTransient<KafkaConsumerFactory>();

            var publishingEventsQueue = new PublishingEventsQueue();
            services.AddSingleton(publishingEventsQueue);
            services.AddHostedService<PublishingService>();

            var serviceProvider = services.BuildServiceProvider();

            var topic = "build.capabilities";
            eventRegistry.Register<ContextAccountCreatedDomainEvent>(
                eventTypeName: "aws_context_account_created",
                topicName: topic,
                eventHandler: serviceProvider.GetRequiredService<IEventHandler<ContextAccountCreatedDomainEvent>>() );

            eventRegistry.Register(
                eventTypeName: "capability_registered",
                topicName: topic,
                eventHandler: serviceProvider.GetRequiredService<IEventHandler<CapabilityRegisteredDomainEvent>>() );
            
            // Published events
            eventRegistry.Register(
                eventTypeName: "k8s_namespace_created_and_aws_arn_connected",
                topicName: topic,
                eventHandler: serviceProvider.GetRequiredService<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>>()
            );

            services.AddTransient<IEventDispatcher, EventDispatcher>();
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