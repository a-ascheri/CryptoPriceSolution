using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoPriceBackend.Providers
{
    public class CoinGeckoProvider : ICurrencyProvider
    {
        private readonly HttpClient _httpClient;

        public CoinGeckoProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public decimal GetPrice(string baseSymbol, string targetSymbol)
        {
            // CoinGecko solo soporta cripto a USD, así que ignoramos targetSymbol
            return GetPriceAsync(baseSymbol).GetAwaiter().GetResult();
        }

        private async Task<decimal> GetPriceAsync(string symbol)
        {
            string id = symbol.ToLower() switch
            {
                "btc" => "bitcoin",
                "eth" => "ethereum",
                _ => symbol.ToLower()
            };

            var url = $"https://api.coingecko.com/api/v3/simple/price?ids={id}&vs_currencies=usd";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return 0.0m;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty(id, out var coin) &&
                coin.TryGetProperty("usd", out var price))
            {
                return price.GetDecimal();
            }

            return 0.0m;
        }
    }
}