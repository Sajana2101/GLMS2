namespace GLMS.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> GetUsdToZarRateAsync();
        decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate);
    }
}
