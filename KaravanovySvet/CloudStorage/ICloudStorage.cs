using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace JenikuvWeb.CloudStorage
{
    public interface ICloudStorage
    {
        string GenerateSignedUrl(string objectName);
        Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage);
        Task DeleteFileAsync(string fileNameForStorage);
    }
}