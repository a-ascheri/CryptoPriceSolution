using CryptoPriceBackend.Models;
using System.Threading.Tasks;

namespace CryptoPriceBackend.Services
{
    public interface IBonosService
    {
        Task<BonosCotizacionResponse?> GetCotizacionAsync(string mercado, string simbolo);
        Task<BonosSerieHistoricaResponse?> GetSerieHistoricaAsync(string mercado, string simbolo, DateTime fechaDesde, DateTime fechaHasta, bool ajustada = true);
    }
}
