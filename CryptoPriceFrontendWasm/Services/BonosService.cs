using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CryptoPriceFrontendWasm.Models;

namespace CryptoPriceFrontendWasm.Services
{
    public class BonosService
    {
        private readonly HttpClient _httpClient;
        public BonosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BonosCotizacionResponse?> GetCotizacionAsync(string mercado, string simbolo)
        {
            return await _httpClient.GetFromJsonAsync<BonosCotizacionResponse>($"api/bonos/cotizacion/{mercado}/{simbolo}");
        }
    }
}
