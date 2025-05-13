
using Microsoft.AspNetCore.Http;

namespace FitnessManagement.Services
{
    public class FileService : IFileService
    {

        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        private readonly string _baseUrl;

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_storagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }




        public FileService(IHttpContextAccessor httpContextAccessor)
        {
            var request = httpContextAccessor.HttpContext?.Request;
            _baseUrl = $"{request?.Scheme}://{request?.Host.Value}";
        }

        public string GetFileUrl(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) ? $"{_baseUrl}{filePath}" : null;
        }

    }
}