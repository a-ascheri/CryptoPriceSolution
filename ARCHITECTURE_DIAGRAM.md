# Diagrama de Arquitectura - Módulo de Bonos

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          NAVEGADOR (Usuario)                            │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │
                                 │ http://localhost:5253/bonos
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                    FRONTEND - Blazor WebAssembly                        │
│                         (Puerto 5253)                                   │
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  Pages/Bonos.razor                                             │   │
│  │  ┌──────────────────────────────────────────────────────────┐ │   │
│  │  │  • Formulario (mercado, símbolo)                         │ │   │
│  │  │  • Botón "Consultar"                                     │ │   │
│  │  │  • Visualización de resultados                           │ │   │
│  │  │  • Manejo de errores                                     │ │   │
│  │  └──────────────────────────────────────────────────────────┘ │   │
│  └────────────────────────────────────────────────────────────────┘   │
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  Models/BonosCotizacionResponse.cs                             │   │
│  │  • DTO para deserializar respuesta                             │   │
│  └────────────────────────────────────────────────────────────────┘   │
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  HttpClient                                                     │   │
│  │  BaseAddress: http://localhost:5166/                           │   │
│  └────────────────────────────────────────────────────────────────┘   │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │
                                 │ HTTP GET
                                 │ /api/bonos/cotizacion/{mercado}/{simbolo}
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                      BACKEND - ASP.NET Core API                         │
│                         (Puerto 5166)                                   │
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  Controllers/BonosController.cs                                │   │
│  │  ┌──────────────────────────────────────────────────────────┐ │   │
│  │  │  [HttpGet("cotizacion/{mercado}/{simbolo}")]            │ │   │
│  │  │  public async Task<ActionResult> GetCotizacion(...)      │ │   │
│  │  │  {                                                        │ │   │
│  │  │      var result = await _bonosService                    │ │   │
│  │  │                         .GetCotizacionAsync(...);         │ │   │
│  │  │      return Ok(result);                                  │ │   │
│  │  │  }                                                        │ │   │
│  │  └──────────────────────────────────────────────────────────┘ │   │
│  └───────────────────────────┬────────────────────────────────────┘   │
│                              │                                         │
│                              │ llama a                                 │
│                              ▼                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  Services/BonosService.cs                                      │   │
│  │  • IBonosService (interfaz)                                    │   │
│  │  • Capa de lógica de negocio                                   │   │
│  │  • Delega al Provider                                          │   │
│  └───────────────────────────┬────────────────────────────────────┘   │
│                              │                                         │
│                              │ llama a                                 │
│                              ▼                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  Providers/BonosProvider.cs                                    │   │
│  │  ┌──────────────────────────────────────────────────────────┐ │   │
│  │  │  • IBonosProvider (interfaz)                             │ │   │
│  │  │  • GetAccessTokenAsync()                                 │ │   │
│  │  │    - Lee credenciales de appsettings.json                │ │   │
│  │  │    - POST /token (OAuth2)                                │ │   │
│  │  │    - Cachea token en memoria                             │ │   │
│  │  │    - Calcula expiración                                  │ │   │
│  │  │    - Renueva automáticamente                             │ │   │
│  │  │                                                           │ │   │
│  │  │  • GetCotizacionAsync(mercado, simbolo)                  │ │   │
│  │  │    - Obtiene token (cache o nuevo)                       │ │   │
│  │  │    - GET /api/{mercado}/Titulos/{simbolo}/Cotizacion     │ │   │
│  │  │    - Headers: Authorization: Bearer {token}              │ │   │
│  │  │    - Deserializa JSON a DTO                              │ │   │
│  │  └──────────────────────────────────────────────────────────┘ │   │
│  └───────────────────────────┬────────────────────────────────────┘   │
│                              │                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  Models/BonosCotizacionResponse.cs                             │   │
│  │  • DTO para mapear respuesta de API externa                    │   │
│  └────────────────────────────────────────────────────────────────┘   │
│                              │                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  appsettings.json                                              │   │
│  │  {                                                              │   │
│  │    "InvertirOnline": {                                         │   │
│  │      "BaseUrl": "https://api.invertironline.com",              │   │
│  │      "Username": "email@example.com",                          │   │
│  │      "Password": "******",                                     │   │
│  │      "GrantType": "password"                                   │   │
│  │    }                                                            │   │
│  │  }                                                              │   │
│  └────────────────────────────────────────────────────────────────┘   │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │
                                 │ HTTPS
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────┐
│              API EXTERNA - InvertirOnline                               │
│              https://api.invertironline.com                             │
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  POST /token                                                    │   │
│  │  • Autentica con username/password                             │   │
│  │  • Retorna access_token (JWT)                                  │   │
│  │  • expires_in: 900-1200 segundos                               │   │
│  └────────────────────────────────────────────────────────────────┘   │
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────┐   │
│  │  GET /api/{Mercado}/Titulos/{Simbolo}/Cotizacion               │   │
│  │  • Requiere: Authorization: Bearer {token}                     │   │
│  │  • Retorna: JSON con cotización del bono                       │   │
│  └────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────┘


═══════════════════════════════════════════════════════════════════════════
                           FLUJO DE DATOS
═══════════════════════════════════════════════════════════════════════════

1. Usuario abre: http://localhost:5253/bonos
2. Ingresa: mercado="argentina", simbolo="AL30"
3. Click en "Consultar"
   ↓
4. Bonos.razor hace: GET http://localhost:5166/api/bonos/cotizacion/argentina/AL30
   ↓
5. BonosController recibe request
   ↓
6. BonosService.GetCotizacionAsync()
   ↓
7. BonosProvider.GetCotizacionAsync()
   ├─ 7a. GetAccessTokenAsync()
   │   ├─ Verifica cache ¿Token válido?
   │   │   ├─ SÍ → Retorna token cacheado
   │   │   └─ NO → Continúa ↓
   │   ├─ Lee credenciales de appsettings.json
   │   ├─ POST https://api.invertironline.com/token
   │   │   Body: username=...&password=...&grant_type=password
   │   ├─ Recibe: { access_token: "...", expires_in: 900 }
   │   ├─ Cachea token y calcula expiración
   │   └─ Retorna access_token
   │
   ├─ 7b. Con token, hace GET a InvertirOnline API
   │   URL: https://api.invertironline.com/api/argentina/Titulos/AL30/Cotizacion
   │   Headers: Authorization: Bearer {token}
   │
   └─ 7c. Deserializa JSON → BonosCotizacionResponse
   ↓
8. Retorna DTO al Controller
   ↓
9. Controller serializa a JSON y retorna HTTP 200
   ↓
10. Bonos.razor recibe y muestra datos


═══════════════════════════════════════════════════════════════════════════
                        GESTIÓN DE TOKENS
═══════════════════════════════════════════════════════════════════════════

Estado Inicial:
_cachedToken = null
_tokenExpiration = DateTime.MinValue

Primera llamada:
├─ GetAccessTokenAsync() verifica cache
├─ No hay token → POST /token
├─ Recibe token con expires_in=900 (15 minutos)
├─ Cachea: _cachedToken = "eyJ..."
├─ Calcula: _tokenExpiration = UtcNow + 840 segundos (14 min)
└─ Retorna token

Llamadas subsecuentes (mientras token válido):
├─ GetAccessTokenAsync() verifica cache
├─ UtcNow < _tokenExpiration → Token válido
└─ Retorna _cachedToken (sin llamada HTTP)

Cuando token expira:
├─ GetAccessTokenAsync() verifica cache
├─ UtcNow >= _tokenExpiration → Token expiró
├─ POST /token (nuevo token)
├─ Actualiza cache y expiración
└─ Retorna nuevo token


═══════════════════════════════════════════════════════════════════════════
                    VENTAJAS DE ESTA ARQUITECTURA
═══════════════════════════════════════════════════════════════════════════

✅ Seguridad:
   • Credenciales nunca expuestas al frontend
   • Token nunca sale del backend
   • Frontend solo conoce su propio backend

✅ Performance:
   • Token cacheado en memoria
   • Evita múltiples llamadas de autenticación
   • Renovación proactiva (antes de expirar)

✅ Mantenibilidad:
   • Arquitectura en capas (Controller → Service → Provider)
   • Separación de responsabilidades
   • Fácil de testear

✅ Escalabilidad:
   • Fácil agregar más providers (otros brokers)
   • Fácil agregar cache distribuido (Redis)
   • Fácil migrar a microservicios

✅ Consistencia:
   • Misma estructura que módulo Crypto
   • Patrones conocidos por el equipo
   • No reinventa la rueda


═══════════════════════════════════════════════════════════════════════════
                    COMPARACIÓN CON MÓDULO CRYPTO
═══════════════════════════════════════════════════════════════════════════

┌─────────────────────┬─────────────────────┬─────────────────────┐
│     COMPONENTE      │       CRYPTO        │       BONOS         │
├─────────────────────┼─────────────────────┼─────────────────────┤
│ Provider Interface  │ ICurrencyProvider   │ IBonosProvider      │
│ Provider Impl       │ CoinGeckoProvider   │ BonosProvider       │
│ Service Interface   │ IPriceService       │ IBonosService       │
│ Service Impl        │ PriceService        │ BonosService        │
│ Controller          │ PricesController    │ BonosController     │
│ Model/DTO           │ PriceResult         │ BonosCotizacion...  │
│ Frontend Page       │ PriceChecker.razor  │ Bonos.razor         │
│ API Externa         │ CoinGecko (libre)   │ InvertirOnline      │
│ Autenticación       │ No requiere         │ OAuth2 Bearer       │
│ Cache               │ No implementado     │ Token cacheado      │
└─────────────────────┴─────────────────────┴─────────────────────┘

Ambos módulos comparten:
• Misma estructura de carpetas
• Mismo patrón de inyección de dependencias
• Mismo flujo de datos
• No se interfieren entre sí
```
