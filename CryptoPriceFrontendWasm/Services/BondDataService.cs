using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CryptoPriceFrontendWasm.Models;

namespace CryptoPriceFrontendWasm.Services
{
    public class BondDataService
    {
        private readonly HttpClient _httpClient;

        public BondDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Obtiene la cotización actual de un bono
        /// </summary>
        public async Task<BonosCotizacionResponse?> GetCotizacionAsync(string mercado, string simbolo)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BonosCotizacionResponse>(
                    $"api/bonos/cotizacion/{mercado}/{simbolo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo cotización: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene la serie histórica de un bono con rango temporal
        /// </summary>
        public async Task<BonosSerieHistoricaResponse?> GetSerieHistoricaAsync(
            string mercado, 
            string simbolo, 
            string rangoTemporal = "1M")
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BonosSerieHistoricaResponse>(
                    $"api/bonos/historico/{mercado}/{simbolo}?rangoTemporal={rangoTemporal}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo serie histórica: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene múltiples series históricas para comparación
        /// </summary>
        public async Task<Dictionary<string, BonosSerieHistoricaResponse?>> GetMultipleSeriesAsync(
            string mercado,
            List<string> simbolos,
            string rangoTemporal = "1M")
        {
            var result = new Dictionary<string, BonosSerieHistoricaResponse?>();
            
            foreach (var simbolo in simbolos)
            {
                var data = await GetSerieHistoricaAsync(mercado, simbolo, rangoTemporal);
                result[simbolo] = data;
            }

            return result;
        }
    }

    /// <summary>
    /// Información de un bono disponible para selección
    /// </summary>
    public class BonoInfo
    {
        public string Simbolo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Mercado { get; set; } = string.Empty;
    }

    /// <summary>
    /// Catálogo de bonos disponibles
    /// </summary>
    public static class BonosCatalogo
    {
        public static List<BonoInfo> BonosArgentinos = new()
        {
            new BonoInfo { Simbolo = "AL30", Nombre = "AL30 - Bono Argentina USD 2030", Pais = "Argentina", Mercado = "argentina" },
            new BonoInfo { Simbolo = "AL29", Nombre = "AL29 - Bono Argentina USD 2029", Pais = "Argentina", Mercado = "argentina" },
            new BonoInfo { Simbolo = "GD30", Nombre = "GD30 - Global 2030", Pais = "Argentina", Mercado = "argentina" },
            new BonoInfo { Simbolo = "AE38", Nombre = "AE38 - Bono Argentina EUR 2038", Pais = "Argentina", Mercado = "argentina" },
            new BonoInfo { Simbolo = "AL35", Nombre = "AL35 - Bono Argentina USD 2035", Pais = "Argentina", Mercado = "argentina" },
            new BonoInfo { Simbolo = "GD35", Nombre = "GD35 - Global 2035", Pais = "Argentina", Mercado = "argentina" },
            new BonoInfo { Simbolo = "GD38", Nombre = "GD38 - Global 2038", Pais = "Argentina", Mercado = "argentina" },
            new BonoInfo { Simbolo = "GD41", Nombre = "GD41 - Global 2041", Pais = "Argentina", Mercado = "argentina" },
            new BonoInfo { Simbolo = "GD46", Nombre = "GD46 - Global 2046", Pais = "Argentina", Mercado = "argentina" },
        };

        public static List<string> Paises = new()
        {
            "Argentina"
        };

        public static List<BonoInfo> GetBonosPorPais(string pais)
        {
            return pais.ToLower() switch
            {
                "argentina" => BonosArgentinos,
                _ => new List<BonoInfo>()
            };
        }
    }
}
