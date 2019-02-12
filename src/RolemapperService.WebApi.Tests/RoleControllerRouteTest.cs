using Xunit;
using RolemapperService.WebApi.Tests.Builders;
using System.Net;
using System.Threading.Tasks;
using RolemapperService.WebApi.Tests.TestDoubles;
using RolemapperService.WebApi.Repositories;
using RolemapperService.WebApi.Services;
using RolemapperService.WebApi.Validators;

namespace RolemapperService.WebApi.Tests
{
    public class RoleControllerRouteTest
    {
        [Fact]
        public async Task GetConfigMap_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<IAwsAuthConfigMapRepository>(new AwsAuthConfigMapRepositoryStub())
                    .WithService<IPersistenceRepository>(new PersistenceRepositoryStub())
                    .Build();

                var response = await client.GetAsync("/api/configmap");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task AddRole_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<IAddRoleRequestValidator>(new AddRoleRequestValidator())
                    .WithService<IConfigMapService>(new ConfigMapServiceStub())
                    .Build();

                var stubInput = @"{
                                ""roleName"": ""ADFS-ViewOnly"",
                                ""roleArn"": ""arn:aws:iam::738063116313:role/ADFS-ViewOnly""
                                }";

                var response = await client.PostAsync("/api/roles", new JsonContent(stubInput));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
