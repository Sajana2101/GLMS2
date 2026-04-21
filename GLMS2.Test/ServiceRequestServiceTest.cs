using GLMS2.Data;
using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.Services;
using GLMS2.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GLMS2.Tests
{
    // Unit tests verifying service request business rules and workflow validation
    public class ServiceRequestServiceTests
    {
        // Uses EF Core in-memory database for isolated testing
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
        [Fact]
        public async Task CreateServiceRequestAsync_ActiveContract_ShouldCreateRequest()
        {
            
            var context = GetDbContext();
            // Test client stored in database
            var client = new Client
            {
                Name = "Test Client",
                Email = "test@client.com",
                Region = "KZN"
            };

            context.Clients.Add(client);
            await context.SaveChangesAsync();
            // Contract must be Active to allow service request creation
            var contract = new Contract
            {
                ClientId = client.ClientId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                Status = ContractStatus.Active,
                ServiceLevel = "Gold",
                ContractType = ContractType.Domestic
            };

            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

          
            var savedContract = await context.Contracts.FirstAsync();
            // Mock external services to isolate business logic
            var currencyServiceMock = new Mock<ICurrencyService>();
            currencyServiceMock.Setup(x => x.GetUsdToZarRateAsync()).ReturnsAsync(18.00m);
            currencyServiceMock.Setup(x => x.ConvertUsdToZar(10m, 18.00m)).Returns(180.00m);

            var mediatorMock = new Mock<IMediator>();

            var service = new ServiceRequestService(context, currencyServiceMock.Object, mediatorMock.Object);

            var model = new ServiceRequestCreateViewModel
            {
                ContractId = savedContract.ContractId,
                Description = "Transport goods",
                CostUSD = 10m
            };
            // Service request created successfully for active contract

            var result = await service.CreateServiceRequestAsync(model);

            
            Assert.NotNull(result);
            Assert.Equal(180.00m, result.CostZAR);
            Assert.Equal(ServiceRequestStatus.Pending, result.Status);
        }

        [Fact]
        public async Task CreateServiceRequestAsync_ExpiredContract_ShouldThrowException()
        {
           
            var context = GetDbContext();
            // Expired contracts should prevent service request creation
            var contract = new Contract
            {
                ClientId = 1,
                StartDate = DateTime.Today.AddDays(-30),
                EndDate = DateTime.Today.AddDays(-1),
                Status = ContractStatus.Expired,
                ServiceLevel = "Silver",
                ContractType = ContractType.Domestic
            };

            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var currencyServiceMock = new Mock<ICurrencyService>();
            var mediatorMock = new Mock<IMediator>();

            var service = new ServiceRequestService(context, currencyServiceMock.Object, mediatorMock.Object);

            var model = new ServiceRequestCreateViewModel
            {
                ContractId = contract.ContractId,
                Description = "Expired contract test",
                CostUSD = 10m
            };
            // Confirms workflow rule enforcement
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateServiceRequestAsync(model));
        }

        [Fact]
        public async Task CreateServiceRequestAsync_OnHoldContract_ShouldThrowException()
        {
            // OnHold contracts should prevent service request creation
            // Arrange
            var context = GetDbContext();

            var contract = new Contract
            {
                ClientId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(10),
                Status = ContractStatus.OnHold,
                ServiceLevel = "Premium",
                ContractType = ContractType.International
            };

            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var currencyServiceMock = new Mock<ICurrencyService>();
            var mediatorMock = new Mock<IMediator>();

            var service = new ServiceRequestService(context, currencyServiceMock.Object, mediatorMock.Object);

            var model = new ServiceRequestCreateViewModel
            {
                ContractId = contract.ContractId,
                Description = "On hold contract test",
                CostUSD = 15m
            };

            // Confirms workflow validation prevents invalid creation
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateServiceRequestAsync(model));
        }
    }
}