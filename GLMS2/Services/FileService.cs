using GLMS2.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GLMS2.Services
{
    public class FileService : IFileService
    {
        public bool IsPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName);

            return !string.IsNullOrWhiteSpace(extension) &&
                   extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string> SavePdfAsync(IFormFile file, string rootPath)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!IsPdf(file))
            {
                throw new InvalidOperationException("Only PDF files are allowed.");
            }

            var uploadsFolder = Path.Combine(rootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
        }
    }
}