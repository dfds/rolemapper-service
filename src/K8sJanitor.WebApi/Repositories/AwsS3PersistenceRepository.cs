using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Transfer;

namespace K8sJanitor.WebApi.Repositories
{
    public class AwsS3PersistenceRepository : IPersistenceRepository
    {
        private readonly ITransferUtility _transferUtility;
        private readonly string _bucketName;
        private readonly string _configMapFileName;

        public AwsS3PersistenceRepository(ITransferUtility transferUtility, string bucketName, string configMapFileName)
        {
            _transferUtility = transferUtility;
            _bucketName = bucketName;
            _configMapFileName = configMapFileName;
        }

        public async Task StoreFile(string content, string contentType = "application/octet-stream")
        {                    
            using (var memoryStream = GenerateStreamFromString(content))
            {
                var req = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    ContentType = contentType,
                    InputStream = memoryStream,
                    Key = _configMapFileName
                };

                await _transferUtility.UploadAsync(req);
            }
        }

        private MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? string.Empty));
        }
    }
}