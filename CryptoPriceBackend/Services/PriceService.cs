using CryptoPriceBackend.Providers;
using System.Collections.Generic;
using System.Linq;

namespace CryptoPriceBackend.Services
{
    public class PriceService : IPriceService
    {
        private readonly IEnumerable<ICurrencyProvider> _providers;

        public PriceService(IEnumerable<ICurrencyProvider> providers)
        {
            _providers = providers;
        }

        public decimal GetCurrentPrice(string baseSymbol, string targetSymbol)
        {
            // Lógica simple: si es cripto usa CoinGecko, si es fiat usa ExchangeRate
            ICurrencyProvider provider = (baseSymbol.ToUpper() switch
            {
                "BTC" or "ETH" => _providers.First(p => p.GetType().Name == "CoinGeckoProvider"),
                _ => _providers.First(p => p.GetType().Name == "ExchangeRateProvider")
            });

            return provider.GetPrice(baseSymbol, targetSymbol);
        }
    }
}