using Microsoft.AspNetCore.Http;
namespace GLMS2.Interfaces
{
    public interface IFileService
    {
        Task<string> SavePdfAsync(IFormFile file, string rootPath);
        bool IsPdf(IFormFile file);
    }
}
