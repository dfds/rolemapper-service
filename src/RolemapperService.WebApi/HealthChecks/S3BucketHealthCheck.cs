using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RolemapperService.WebApi.HealthChecks
{
    public class S3BucketHealthCheck : IHealthCheck
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly string _bucketName;

        public S3BucketHealthCheck(IAmazonS3 amazonS3, string bucketName)
        {
            _amazonS3 = amazonS3;
            _bucketName = bucketName;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken()
        )
        {
            var listObjectsRequest = new ListObjectsRequest {BucketName = _bucketName};
            try
            {
                await _amazonS3.ListObjectsAsync(listObjectsRequest);
            }
            catch (AmazonS3Exception e) when (e.ErrorCode == "NoSuchBucket")
            {
                return HealthCheckResult.Unhealthy(description: $"S3 bucket {_bucketName} does not exist");
            }

            return HealthCheckResult.Healthy(description: "S3 bucket responding");
        }
    }
}