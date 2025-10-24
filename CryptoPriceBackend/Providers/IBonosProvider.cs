using CryptoPriceBackend.Models;
using System.Threading.Tasks;

namespace CryptoPriceBackend.Providers
{
    public interface IBonosProvider
    {
        Task<BonosCotizacionResponse?> GetCotizacionAsync(string mercado, string simbolo);
    }
}
