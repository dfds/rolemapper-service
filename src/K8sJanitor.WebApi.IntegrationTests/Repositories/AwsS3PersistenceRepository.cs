using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Xunit;

namespace K8sJanitor.WebApi.IntegrationTests.Repositories
{
    // Not currently representative of actual setup, merely just testing that the repository works
    public class AwsS3PersistenceRepository
    {
        [Fact]
        public async Task StoreFile()
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName("eu-central-1");
            var s3Client = new AmazonS3Client(regionEndpoint);
            var transferUtility = new TransferUtility(s3Client);
            var awsS3PersistenceRepository = new WebApi.Repositories.AwsS3PersistenceRepository(transferUtility, "k8sjanitor-integrationtest", "test-cm");
            await awsS3PersistenceRepository.StoreFile("Test sample data", "text/plain");

        }
    }
}