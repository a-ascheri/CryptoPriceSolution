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
    }
}
