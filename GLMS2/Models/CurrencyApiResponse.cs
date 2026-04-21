using System.Text.Json.Serialization;

namespace GLMS2.Models
{
    // Represents the response structure returned by the currency API
    // Used to extract exchange rate values (e.g., USD to ZAR)
    public class CurrencyApiResponse
    {
        // Maps the "rates" section from the JSON response
        // Stores currency codes and their exchange rates
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal>? Rates { get; set; }
    }
}