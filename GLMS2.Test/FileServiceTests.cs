using GLMS2.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace GLMS2.Tests
{
    public class FileServiceTests
    {
        [Fact]
        public void IsPdf_ValidPdfFile_ShouldReturnTrue()
        {
           
            var service = new FileService();
            var file = CreateMockFormFile("contract.pdf", "Dummy PDF content");

           
            var result = service.IsPdf(file);

           
            Assert.True(result);
        }

        [Fact]
        public void IsPdf_ExeFile_ShouldReturnFalse()
        {
          
            var service = new FileService();
            var file = CreateMockFormFile("virus.exe", "Bad file");

           
            var result = service.IsPdf(file);

          
            Assert.False(result);
        }

        [Fact]
        public async Task SavePdfAsync_InvalidFileType_ShouldThrowException()
        {
            
            var service = new FileService();
            var file = CreateMockFormFile("virus.exe", "Bad file");
            var rootPath = Path.GetTempPath();

           
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
    service.SavePdfAsync(file, rootPath));
        }

        private IFormFile CreateMockFormFile(string fileName, string content)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "Data", fileName);
        }
    }
}