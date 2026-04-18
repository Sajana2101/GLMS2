using GLMS2.Interfaces;
using GLMS2.Models;
using System.Text.Json;

namespace GLMS2.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var url = "https://open.er-api.com/v6/latest/USD";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<CurrencyApiResponse>(json);

                if (apiResponse?.Rates == null || !apiResponse.Rates.ContainsKey("ZAR"))
                {
                    throw new InvalidOperationException("ZAR exchange rate not found.");
                }

                return apiResponse.Rates["ZAR"];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve exchange rate.", ex);
            }
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            if (usdAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(usdAmount), "USD amount cannot be negative.");
            }

            if (exchangeRate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exchangeRate), "Exchange rate must be greater than zero.");
            }

            return Math.Round(usdAmount * exchangeRate, 2);
        }
    }
}