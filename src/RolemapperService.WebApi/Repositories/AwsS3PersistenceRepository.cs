using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Transfer;

namespace RolemapperService.WebApi.Repositories
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

        public async Task StoreFile(string content)
        {                    
            using (var memoryStream = GenerateStreamFromString(content))
            {
                await _transferUtility.UploadAsync(stream: memoryStream,
                                                   bucketName: _bucketName,
                                                   key: _configMapFileName);
            }
        }

        private MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? string.Empty));
        }
    }
}