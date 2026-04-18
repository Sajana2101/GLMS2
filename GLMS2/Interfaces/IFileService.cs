using Microsoft.AspNetCore.Http;
namespace GLMS.Interfaces
{
    public interface IFileService
    {
        Task<string> SavePdfAsync(IFormFile file, string rootPath);
        bool IsPdf(IFormFile file);
    }
}
