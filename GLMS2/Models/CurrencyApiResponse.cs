using System.Text.Json.Serialization;

namespace GLMS.Models
{
    public class CurrencyApiResponse
    {
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal>? Rates { get; set; }
    }
}