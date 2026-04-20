using GLMS2.Services;
using Xunit;

namespace GLMS2.Tests
{
    public class CurrencyServiceTests
    {
        [Fact]
        public void ConvertUsdToZar_ShouldReturnCorrectAmount()
        {
            
            var service = new CurrencyService(new HttpClient());
            decimal usdAmount = 10m;
            decimal exchangeRate = 18.50m;

          
            var result = service.ConvertUsdToZar(usdAmount, exchangeRate);

            Assert.Equal(185.00m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ZeroUsd_ShouldReturnZero()
        {
           
            var service = new CurrencyService(new HttpClient());

          
            var result = service.ConvertUsdToZar(0m, 18.50m);

           
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ConvertUsdToZar_NegativeUsd_ShouldThrowException()
        {
           
            var service = new CurrencyService(new HttpClient());

            
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.ConvertUsdToZar(-5m, 18.50m));
        }

        [Fact]
        public void ConvertUsdToZar_InvalidRate_ShouldThrowException()
        {
          
            var service = new CurrencyService(new HttpClient());

          
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.ConvertUsdToZar(10m, 0m));
        }
    }
}