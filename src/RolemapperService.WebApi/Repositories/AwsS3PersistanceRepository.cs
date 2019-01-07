using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Transfer;

namespace RolemapperService.WebApi.Repositories
{
    public class AwsS3PersistanceRepository : IPersistanceRepository
    {
        private readonly ITransferUtility _transferUtility;
        private readonly string _bucketName;

        public AwsS3PersistanceRepository(ITransferUtility transferUtility, string bucketName)
        {
            _transferUtility = transferUtility;
            _bucketName = bucketName;
        }

        public async Task StoreFile(string fileName, string content)
        {                    
            using (var memoryStream = GenerateStreamFromString(content))
            {
                await _transferUtility.UploadAsync(stream: memoryStream,
                                                   bucketName: _bucketName,
                                                   key: fileName);
            }
        }

        private MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? string.Empty));
        }
    }
}