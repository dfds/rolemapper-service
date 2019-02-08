using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using k8s;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RolemapperService.WebApi.Models.ExternalEvents;
using RolemapperService.WebApi.Repositories;
using RolemapperService.WebApi.Repositories.Kubernetes;
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

            if (_hostingEnvironment.IsDevelopment())
            {
                services.AddTransient<IKubernetes>(serviceProvider => 
                    new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile()));

                services.AddTransient<IConfigMapPersistanceService, ConfigMapPersistanceServiceStub>();
            }
            else
            {
                services.AddTransient<IKubernetes>(serviceProvider => new Kubernetes(KubernetesClientConfiguration.InClusterConfig()));
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

                services.AddTransient<IPersistanceRepository>(serviceProvider => new AwsS3PersistanceRepository(
                    transferUtility: serviceProvider.GetRequiredService<ITransferUtility>(),
                    bucketName: Configuration["AWS_S3_BUCKET_NAME_CONFIG_MAP"]
                ));

                services.AddTransient<IConfigMapPersistanceService>(serviceProvider => new ConfigMapPersistenceService(
                    persistenceRepository: serviceProvider.GetRequiredService<IPersistanceRepository>(),
                    configMapFileName: Configuration["CONFIG_MAP_FILE_NAME"]
                ));
            }


            services.AddTransient<IConfigMapService, ConfigMapService>();
            services.AddTransient<IAwsAuthConfigMapRepository, AwsAuthConfigMapRepository>();
            services.AddTransient<IAddRoleRequestValidator, AddRoleRequestValidator>();
            services.AddTransient<IAddNamespaceRequestValidator, AddNamespaceRequestValidator>();
            services.AddTransient<INamespaceRespoitory, NamespaceRespoitory>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRoleBindingRepository, RoleBindingRepository>();

            // Event handlers
            services.AddTransient<IEventHandler<CapabilityRegisteredEvent>, CapabilityRegisteredEventHandler>();
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