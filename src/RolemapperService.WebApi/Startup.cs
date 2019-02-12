﻿using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using k8s;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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