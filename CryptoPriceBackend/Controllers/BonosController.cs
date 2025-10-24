using Microsoft.AspNetCore.Mvc;
using CryptoPriceBackend.Services;
using CryptoPriceBackend.Models;
using System.Threading.Tasks;

namespace CryptoPriceBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BonosController : ControllerBase
    {
        private readonly IBonosService _bonosService;

        public BonosController(IBonosService bonosService)
        {
            _bonosService = bonosService;
        }

        [HttpGet("cotizacion/{mercado}/{simbolo}")]
        public async Task<ActionResult<BonosCotizacionResponse>> GetCotizacion(string mercado, string simbolo)
        {
            var result = await _bonosService.GetCotizacionAsync(mercado, simbolo);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
