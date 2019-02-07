using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using k8s;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddTransient<IKubernetes>(serviceProvider =>
                        {
                            var config =
                            _hostingEnvironment.IsDevelopment()
                                ? KubernetesClientConfiguration.BuildConfigFromConfigFile()
                                : KubernetesClientConfiguration.InClusterConfig();

                            return new Kubernetes(config);
                        });

            services.AddTransient<AWSCredentials>(serviceProvider => new BasicAWSCredentials(
                accessKey: Configuration["S3_AWS_ACCESS_KEY_ID"],
                secretKey: Configuration["S3_AWS_SECRET_ACCESS_KEY"]
            ));

            services.AddTransient(serviceProvider => RegionEndpoint.GetBySystemName(Configuration["S3_AWS_REGION"]));

            services.AddTransient<IAmazonS3>(serviceProvider => new AmazonS3Client(
                credentials: serviceProvider.GetRequiredService<AWSCredentials>(),
                region: serviceProvider.GetRequiredService<RegionEndpoint>()
            ));

            services.AddTransient<ITransferUtility>(serviceProvider => new TransferUtility(
                s3Client: serviceProvider.GetRequiredService<IAmazonS3>()
            ));

            services.AddTransient<IConfigMapService, ConfigMapService>();
            services.AddTransient<IKubernetesService, KubernetesService>();
            services.AddTransient<IAwsAuthConfigMapRepository, AwsAuthConfigMapRepository>();
            services.AddTransient<IAddRoleRequestValidator, AddRoleRequestValidator>();
            services.AddTransient<IAddNamespaceRequestValidator, AddNamespaceRequestValidator>();
            services.AddTransient<NamespaceRespoitory>();
            services.AddTransient<RoleRepository>();
            services.AddTransient<RoleBindingRepository>();
            services.AddTransient<IRoleService, RoleService>();
            
            
            services.AddTransient<IPersistanceRepository>(serviceProvider => new AwsS3PersistanceRepository(
                transferUtility: serviceProvider.GetRequiredService<ITransferUtility>(),
                bucketName: Configuration["AWS_S3_BUCKET_NAME_CONFIG_MAP"]
            ));

            services.AddTransient<IConfigMapPersistanceService>(serviceProvider => new ConfigMapPersistanceService(
                persistanceRepository: serviceProvider.GetRequiredService<IPersistanceRepository>(),
                configMapFileName: Configuration["CONFIG_MAP_FILE_NAME"]
            ));
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
