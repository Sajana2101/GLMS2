using GLMS2.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace GLMS2.Tests
{
    // Unit tests for validating file upload rules
    public class FileServiceTests
    {
        [Fact]
        public void IsPdf_ValidPdfFile_ShouldReturnTrue()
        {
            // Ensures only PDF files are allowed
            var service = new FileService();
            var file = CreateMockFormFile("contract.pdf", "Dummy PDF content");

           
            var result = service.IsPdf(file);

           
            Assert.True(result);
        }

        [Fact]
        public void IsPdf_ExeFile_ShouldReturnFalse()
        {
            // Prevents non-PDF files from being saved
            var service = new FileService();
            var file = CreateMockFormFile("virus.exe", "Bad file");

           
            var result = service.IsPdf(file);

          
            Assert.False(result);
        }

        [Fact]
        public async Task SavePdfAsync_InvalidFileType_ShouldThrowException()
        {
            // Prevents non-PDF files from being saved
            var service = new FileService();
            var file = CreateMockFormFile("virus.exe", "Bad file");
            var rootPath = Path.GetTempPath();

           
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
    service.SavePdfAsync(file, rootPath));
        }
        // Creates a mock file for testing upload functionality
        private IFormFile CreateMockFormFile(string fileName, string content)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "Data", fileName);
        }
    }
}