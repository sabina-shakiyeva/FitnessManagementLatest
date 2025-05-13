using Microsoft.AspNetCore.Http;

namespace FitnessManagement.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
        string GetFileUrl(string filePath);


    }
}
