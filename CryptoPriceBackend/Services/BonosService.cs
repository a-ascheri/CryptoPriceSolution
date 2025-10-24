using CryptoPriceBackend.Models;
using CryptoPriceBackend.Providers;
using System.Threading.Tasks;

namespace CryptoPriceBackend.Services
{
    public class BonosService : IBonosService
    {
        private readonly IBonosProvider _provider;
        public BonosService(IBonosProvider provider)
        {
            _provider = provider;
        }
        public async Task<BonosCotizacionResponse?> GetCotizacionAsync(string mercado, string simbolo)
        {
            return await _provider.GetCotizacionAsync(mercado, simbolo);
        }
    }
}
