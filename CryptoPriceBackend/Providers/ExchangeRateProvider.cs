using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoPriceBackend.Providers
{
    public class ExchangeRateProvider : ICurrencyProvider
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public decimal GetPrice(string baseSymbol, string targetSymbol)
        {
            return GetPriceAsync(baseSymbol, targetSymbol).GetAwaiter().GetResult();
        }

        private async Task<decimal> GetPriceAsync(string baseSymbol, string targetSymbol)
        {
            var url = $"https://open.er-api.com/v6/latest/{baseSymbol.ToUpper()}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return 0.0m;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("rates", out var rates) &&
                rates.TryGetProperty(targetSymbol.ToUpper(), out var price))
            {
                return price.GetDecimal();
            }

            return 0.0m;
        }
    }
}