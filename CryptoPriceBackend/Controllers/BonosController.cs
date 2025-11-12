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

        /// <summary>
        /// Obtiene la serie histórica de precios de un bono
        /// </summary>
        /// <param name="mercado">Mercado del bono (ej: argentina)</param>
        /// <param name="simbolo">Símbolo del bono (ej: AL30)</param>
        /// <param name="rangoTemporal">Rango temporal: 1M, 3M, 6M, 1A, 5A, MAX</param>
        /// <param name="fechaDesde">Fecha desde (opcional, si no se usa rangoTemporal)</param>
        /// <param name="fechaHasta">Fecha hasta (opcional, si no se usa rangoTemporal)</param>
        /// <param name="ajustada">Si la serie debe ser ajustada (default: true)</param>
        [HttpGet("historico/{mercado}/{simbolo}")]
        public async Task<ActionResult<BonosSerieHistoricaResponse>> GetSerieHistorica(
            string mercado, 
            string simbolo,
            [FromQuery] string? rangoTemporal = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] bool ajustada = true)
        {
            DateTime desde;
            DateTime hasta = fechaHasta ?? DateTime.Now;

            // Determinar el rango de fechas
            if (!string.IsNullOrWhiteSpace(rangoTemporal))
            {
                desde = rangoTemporal.ToUpper() switch
                {
                    "1M" => hasta.AddMonths(-1),
                    "3M" => hasta.AddMonths(-3),
                    "6M" => hasta.AddMonths(-6),
                    "1A" or "1Y" => hasta.AddYears(-1),
                    "5A" or "5Y" => hasta.AddYears(-5),
                    "MAX" => hasta.AddYears(-20), // Máximo 20 años
                    _ => hasta.AddMonths(-1) // Default 1 mes
                };
            }
            else if (fechaDesde.HasValue)
            {
                desde = fechaDesde.Value;
            }
            else
            {
                // Por defecto, último mes
                desde = hasta.AddMonths(-1);
            }

            var result = await _bonosService.GetSerieHistoricaAsync(mercado, simbolo, desde, hasta, ajustada);
            
            if (result == null)
                return NotFound(new { message = $"No se encontraron datos históricos para {simbolo}" });
            
            return Ok(result);
        }
    }
}
