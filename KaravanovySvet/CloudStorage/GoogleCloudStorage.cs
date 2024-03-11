using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using static JenikuvWeb.CloudStorage.ICloudStorage;

namespace JenikuvWeb.CloudStorage
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly GoogleCredential googleCredential;
        private readonly StorageClient storageClient;
        private readonly string bucketName;

        public GoogleCloudStorage(IConfiguration configuration)
        {
            googleCredential = GoogleCredential.FromFile(configuration.GetValue<string>("GoogleCredentialFile"));
            storageClient = StorageClient.Create(googleCredential);
            bucketName = configuration.GetValue<string>("GoogleCloudStorageBucket");
        }

        public string GenerateSignedUrl(string objectName)
        {
            UrlSigner urlSigner = UrlSigner.FromCredential(googleCredential);
            string url = urlSigner.Sign(bucketName, objectName, TimeSpan.FromMinutes(1), HttpMethod.Get);

            return url;
        }

        public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var dataObject = await storageClient.UploadObjectAsync(bucketName, fileNameForStorage, null, memoryStream);
                return dataObject.MediaLink;
            }
        }

        public async Task DeleteFileAsync(string fileNameForStorage)
        {
            await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
        }

        public async Task<MemoryStream> DownloadObjectAsync(string bucketName, string objectName)
        {
            var memoryStream = new MemoryStream();
            try
            {
                await storageClient.DownloadObjectAsync(bucketName, objectName, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin); // Reset stream position to beginning
                return memoryStream;
            }
            catch (Exception ex)
            {
                // Log error or handle it accordingly
                throw new Exception($"Error downloading object {objectName} from bucket {bucketName}: {ex.Message}", ex);
            }
        }
    }    
}