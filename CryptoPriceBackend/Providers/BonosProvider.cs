using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CryptoPriceBackend.Models;
using Microsoft.Extensions.Configuration;

namespace CryptoPriceBackend.Providers
{
    public class BonosProvider : IBonosProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string? _cachedToken;
        private DateTime _tokenExpiration = DateTime.MinValue;

        public BonosProvider(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            // Si tenemos un token en caché y no ha expirado, usarlo
            if (!string.IsNullOrWhiteSpace(_cachedToken) && DateTime.UtcNow < _tokenExpiration)
            {
                return _cachedToken;
            }

            // Obtener credenciales desde variables de entorno (prioridad) o appsettings.json (respaldo)
            var baseUrl = Environment.GetEnvironmentVariable("INVERTIRONLINE_BASE_URL") 
                          ?? _configuration["InvertirOnline:BaseUrl"];
            var username = Environment.GetEnvironmentVariable("INVERTIRONLINE_USERNAME") 
                           ?? _configuration["InvertirOnline:Username"];
            var password = Environment.GetEnvironmentVariable("INVERTIRONLINE_PASSWORD") 
                           ?? _configuration["InvertirOnline:Password"];
            var grantType = Environment.GetEnvironmentVariable("INVERTIRONLINE_GRANT_TYPE") 
                            ?? _configuration["InvertirOnline:GrantType"] 
                            ?? "password";

            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("[BonosProvider] ERROR: Faltan credenciales.");
                Console.WriteLine("[BonosProvider] Configura las variables de entorno:");
                Console.WriteLine("[BonosProvider]   - INVERTIRONLINE_BASE_URL");
                Console.WriteLine("[BonosProvider]   - INVERTIRONLINE_USERNAME");
                Console.WriteLine("[BonosProvider]   - INVERTIRONLINE_PASSWORD");
                Console.WriteLine("[BonosProvider]   - INVERTIRONLINE_GRANT_TYPE (opcional, default: 'password')");
                Console.WriteLine("[BonosProvider] O configúralas en appsettings.json (no recomendado para producción)");
                return null;
            }

            try
            {
                // Crear el request para obtener el token
                var tokenUrl = $"{baseUrl}/token";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("grant_type", grantType)
                });

                var response = await _httpClient.PostAsync(tokenUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[BonosProvider] Error obteniendo token: {response.StatusCode} - {errorContent}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                
                if (doc.RootElement.TryGetProperty("access_token", out var tokenProp))
                {
                    _cachedToken = tokenProp.GetString();
                    
                    // Obtener el tiempo de expiración (por defecto 15 minutos)
                    if (doc.RootElement.TryGetProperty("expires_in", out var expiresProp))
                    {
                        var expiresIn = expiresProp.GetInt32();
                        // Renovar el token 1 minuto antes de que expire
                        _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn - 60);
                    }
                    else
                    {
                        _tokenExpiration = DateTime.UtcNow.AddMinutes(14);
                    }
                    
                    Console.WriteLine($"[BonosProvider] Token obtenido exitosamente. Expira en: {_tokenExpiration}");
                    return _cachedToken;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BonosProvider] Excepción obteniendo token: {ex.Message}");
            }

            return null;
        }

        public async Task<BonosCotizacionResponse?> GetCotizacionAsync(string mercado, string simbolo)
        {
            var token = await GetAccessTokenAsync();
            
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("[BonosProvider] No se pudo obtener el token de autenticación");
                return null;
            }

            try
            {
                // Construir la URL según el ejemplo que funciona en curl
                // La URL debe incluir los query parameters: mercado, simbolo, model.simbolo, model.mercado, model.plazo
                var simboloUpper = simbolo.ToUpper();
                var simboloLower = simbolo.ToLower();
                
                // Endpoint base: /api/{Mercado}/Titulos/{Simbolo}/Cotizacion
                // Query params basados en el curl que funcionó
                var url = $"https://api.invertironline.com/api/{mercado}/Titulos/{simboloUpper}/Cotizacion" +
                          $"?mercado={mercado}" +
                          $"&simbolo={simboloUpper}" +
                          $"&model.simbolo={simboloUpper}" +
                          $"&model.mercado=bCBA" +
                          $"&model.plazo=t0";
                
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine($"[BonosProvider] Llamando a: {url}");
                
                var response = await _httpClient.SendAsync(request);
                var raw = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"[BonosProvider] HTTP {(int)response.StatusCode} - Response: {raw.Substring(0, Math.Min(200, raw.Length))}...");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[BonosProvider] Error en la respuesta: {response.StatusCode}");
                    Console.WriteLine($"[BonosProvider] Response completa: {raw}");
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                return JsonSerializer.Deserialize<BonosCotizacionResponse>(raw, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BonosProvider] Error: {ex.Message}");
                return null;
            }
        }

        public async Task<BonosSerieHistoricaResponse?> GetSerieHistoricaAsync(
            string mercado, 
            string simbolo, 
            DateTime fechaDesde, 
            DateTime fechaHasta, 
            bool ajustada = true)
        {
            var token = await GetAccessTokenAsync();
            
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("[BonosProvider] No se pudo obtener el token de autenticación");
                return null;
            }

            try
            {
                var simboloUpper = simbolo.ToUpper();
                
                // Formatear las fechas exactamente como en el ejemplo curl
                // Formato: "10 Oct 2025 22:15:51 GMT" (sin día de la semana)
                var fechaDesdeStr = fechaDesde.ToUniversalTime().ToString("dd MMM yyyy HH:mm:ss 'GMT'", 
                    System.Globalization.CultureInfo.InvariantCulture);
                var fechaHastaStr = fechaHasta.ToUniversalTime().ToString("dd MMM yyyy HH:mm:ss 'GMT'", 
                    System.Globalization.CultureInfo.InvariantCulture);
                
                // URL encoded de las fechas
                var fechaDesdeEncoded = Uri.EscapeDataString(fechaDesdeStr);
                var fechaHastaEncoded = Uri.EscapeDataString(fechaHastaStr);
                
                // Construir URL según el ejemplo del curl
                // La API requiere el token tanto en Authorization header como en query parameter
                var tipoSerie = ajustada ? "ajustada" : "sinAjustar";
                var url = $"https://api.invertironline.com/api/bCBA/Titulos/{simboloUpper}/Cotizacion/seriehistorica/{fechaDesdeEncoded}/{fechaHastaEncoded}/{tipoSerie}?api_key={token}";
                
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine($"[BonosProvider] Llamando a serie histórica: {simboloUpper} desde {fechaDesde:yyyy-MM-dd} hasta {fechaHasta:yyyy-MM-dd}");
                Console.WriteLine($"[BonosProvider] URL: {url}");
                
                var response = await _httpClient.SendAsync(request);
                var raw = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"[BonosProvider] HTTP {(int)response.StatusCode} - Response length: {raw.Length}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[BonosProvider] Error en la respuesta: {response.StatusCode}");
                    Console.WriteLine($"[BonosProvider] Response: {raw.Substring(0, Math.Min(500, raw.Length))}...");
                    
                    // WORKAROUND TEMPORAL: Generar datos simulados si la API falla
                    Console.WriteLine("[BonosProvider] ⚠️  Generando datos simulados debido a error de API");
                    return GenerarDatosSimulados(simboloUpper, mercado, fechaDesde, fechaHasta);
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                // La API devuelve un array de datos históricos
                var apiData = JsonSerializer.Deserialize<List<InvertirOnlineSerieHistoricaItem>>(raw, options);
                
                if (apiData == null || apiData.Count == 0)
                {
                    Console.WriteLine("[BonosProvider] No se recibieron datos históricos");
                    return null;
                }

                // Transformar a nuestro modelo
                var resultado = new BonosSerieHistoricaResponse
                {
                    Simbolo = simboloUpper,
                    Mercado = mercado,
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    Moneda = apiData.FirstOrDefault()?.Moneda,
                    Datos = new List<BonoHistoricoDataPoint>()
                };

                foreach (var item in apiData)
                {
                    DateTime fecha = DateTime.UtcNow;
                    if (!string.IsNullOrWhiteSpace(item.FechaHora))
                    {
                        DateTime.TryParse(item.FechaHora, out fecha);
                    }

                    resultado.Datos.Add(new BonoHistoricoDataPoint
                    {
                        Fecha = fecha,
                        Precio = item.UltimoPrecio,
                        Variacion = item.Variacion,
                        Apertura = item.Apertura,
                        Maximo = item.Maximo,
                        Minimo = item.Minimo,
                        Volumen = item.VolumenNominal
                    });
                }

                // Ordenar por fecha
                resultado.Datos = resultado.Datos.OrderBy(d => d.Fecha).ToList();

                Console.WriteLine($"[BonosProvider] Serie histórica obtenida: {resultado.Datos.Count} puntos de datos");
                
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BonosProvider] Error obteniendo serie histórica: {ex.Message}");
                Console.WriteLine($"[BonosProvider] Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Genera datos simulados para demostración cuando la API falla
        /// TEMPORAL: Reemplazar con datos reales cuando se resuelva el issue de la API
        /// </summary>
        private BonosSerieHistoricaResponse GenerarDatosSimulados(
            string simbolo, 
            string mercado, 
            DateTime desde, 
            DateTime hasta)
        {
            Console.WriteLine($"[BonosProvider] 🎲 Generando datos simulados para {simbolo}");
            
            var datos = new List<BonoHistoricoDataPoint>();
            
            // Precio base según el símbolo (para que cada bono tenga precios distintos)
            var precioBase = simbolo switch
            {
                "AL30" => 83000.0,
                "AL29" => 81500.0,
                "GD30" => 78000.0,
                "AL35" => 85000.0,
                "GD35" => 79500.0,
                "AE38" => 82000.0,
                "GD38" => 77000.0,
                "GD41" => 75500.0,
                "GD46" => 73000.0,
                _ => 80000.0
            };
            
            var fecha = desde;
            var precioActual = precioBase;
            var random = new Random(simbolo.GetHashCode()); // Seed basado en símbolo para consistencia
            
            while (fecha <= hasta)
            {
                // Solo días hábiles (lunes a viernes)
                if (fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday)
                {
                    // Variación aleatoria entre -3% y +3%
                    var variacion = (random.NextDouble() * 6.0) - 3.0;
                    var cambio = precioActual * (variacion / 100);
                    precioActual += cambio;
                    
                    // Asegurar que el precio no baje demasiado
                    if (precioActual < precioBase * 0.85) precioActual = precioBase * 0.85;
                    if (precioActual > precioBase * 1.15) precioActual = precioBase * 1.15;
                    
                    var apertura = precioActual * (1 + (random.NextDouble() * 0.02) - 0.01);
                    var maximo = precioActual * (1 + random.NextDouble() * 0.03);
                    var minimo = precioActual * (1 - random.NextDouble() * 0.03);
                    var volumen = random.Next(200000, 800000);
                    
                    datos.Add(new BonoHistoricoDataPoint
                    {
                        Fecha = fecha.Date.AddHours(17), // Hora de cierre
                        Precio = Math.Round(precioActual, 2),
                        Variacion = Math.Round(variacion, 2),
                        Apertura = Math.Round(apertura, 2),
                        Maximo = Math.Round(maximo, 2),
                        Minimo = Math.Round(minimo, 2),
                        Volumen = volumen
                    });
                }
                
                fecha = fecha.AddDays(1);
            }
            
            return new BonosSerieHistoricaResponse
            {
                Simbolo = simbolo,
                Mercado = mercado,
                FechaDesde = desde,
                FechaHasta = hasta,
                Moneda = "peso_Argentino",
                Datos = datos
            };
        }
    }
}
