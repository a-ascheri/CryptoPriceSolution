using CryptoPriceBackend.Models;
using CryptoPriceBackend.Providers;
using System.Threading.Tasks;

namespace CryptoPriceBackend.Services
{
    public class BonosService : IBonosService
    {
        private readonly IBonosProvider _provider;
        private readonly BondCacheService _cache;

        public BonosService(IBonosProvider provider, BondCacheService cache)
        {
            _provider = provider;
            _cache = cache;
        }

        public async Task<BonosCotizacionResponse?> GetCotizacionAsync(string mercado, string simbolo)
        {
            return await _provider.GetCotizacionAsync(mercado, simbolo);
        }

        public async Task<BonosSerieHistoricaResponse?> GetSerieHistoricaAsync(
            string mercado, 
            string simbolo, 
            DateTime fechaDesde, 
            DateTime fechaHasta, 
            bool ajustada = true)
        {
            // Intentar obtener del caché
            var cacheKey = BondCacheService.GenerateKey(mercado, simbolo, fechaDesde, fechaHasta);
            var cachedData = _cache.Get(cacheKey);
            
            if (cachedData != null)
            {
                return cachedData;
            }

            // Si no está en caché, obtener del provider
            var data = await _provider.GetSerieHistoricaAsync(mercado, simbolo, fechaDesde, fechaHasta, ajustada);
            
            if (data != null)
            {
                _cache.Set(cacheKey, data);
            }

            return data;
        }
    }
}
