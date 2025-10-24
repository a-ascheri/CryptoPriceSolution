# Resumen Técnico - Módulo de Bonos

## ✅ Implementación Completada

Se ha creado un módulo completo de Bonos para consultar cotizaciones de la API de InvertirOnline, siguiendo la misma arquitectura del módulo de Crypto.

---

## 📁 Archivos Creados/Modificados

### Backend

#### ✅ Creados (ya existían pero se mejoraron):
1. **Providers/IBonosProvider.cs** ✓
   - Interfaz del Provider

2. **Providers/BonosProvider.cs** ✓
   - Gestión automática de tokens OAuth2
   - Cache de tokens con renovación automática
   - Llamadas HTTP autenticadas a InvertirOnline API
   - Manejo de errores y logging

3. **Services/IBonosService.cs** ✓
   - Interfaz del Service

4. **Services/BonosService.cs** ✓
   - Capa de servicio entre Controller y Provider

5. **Controllers/BonosController.cs** ✓
   - Endpoint: `GET /api/bonos/cotizacion/{mercado}/{simbolo}`

6. **Models/BonosCotizacionResponse.cs** ✓
   - DTO para la respuesta de la API

#### ✅ Modificados:
1. **appsettings.json** ✓
   ```json
   "InvertirOnline": {
     "BaseUrl": "https://api.invertironline.com",
     "Username": "ascheriagustin@gmail.com",
     "Password": "Invertironline123.",
     "GrantType": "password"
   }
   ```

2. **Program.cs** ✓
   - Registrado IBonosProvider
   - Registrado IBonosService
   - Inyección de IConfiguration para gestión de credenciales

### Frontend

#### ✅ Modificados:
1. **Pages/Bonos.razor** ✓
   - Interfaz para consultar bonos
   - Formulario con mercado y símbolo
   - Visualización de resultados
   - Manejo de errores

2. **Models/BonosCotizacionResponse.cs** ✓
   - DTO para deserializar respuesta del backend

3. **Program.cs** ✓
   - HttpClient configurado con BaseAddress = http://localhost:5166/

---

## 🏗️ Arquitectura

### Flujo de Datos

```
Frontend (Bonos.razor)
    ↓ HTTP GET
    ↓ http://localhost:5166/api/bonos/cotizacion/{mercado}/{simbolo}
    ↓
Backend (BonosController)
    ↓ llama a
    ↓
Service (BonosService)
    ↓ llama a
    ↓
Provider (BonosProvider)
    ↓ 1. Obtiene token OAuth2 (cache automático)
    ↓ 2. GET https://api.invertironline.com/api/{mercado}/Titulos/{simbolo}/Cotizacion
    ↓ 3. Headers: Authorization: Bearer {token}
    ↓
InvertirOnline API
```

---

## 🔐 Gestión de Autenticación

### BonosProvider - Token Management

El Provider implementa un sistema inteligente de gestión de tokens:

```csharp
private string? _cachedToken;
private DateTime _tokenExpiration = DateTime.MinValue;

private async Task<string?> GetAccessTokenAsync()
{
    // 1. Verificar si hay token en caché y no ha expirado
    if (!string.IsNullOrWhiteSpace(_cachedToken) && 
        DateTime.UtcNow < _tokenExpiration)
    {
        return _cachedToken;
    }

    // 2. Obtener credenciales de appsettings.json
    // 3. POST /token con username, password, grant_type
    // 4. Cachear el token
    // 5. Calcular expiración (expires_in - 60 segundos)
    
    return _cachedToken;
}
```

**Ventajas:**
- ✅ No requiere archivo `bonos_token.json`
- ✅ Token se renueva automáticamente
- ✅ No expone credenciales al frontend
- ✅ Cache en memoria para mejor rendimiento

---

## 🌐 Endpoints

### Backend API

**Base URL:** `http://localhost:5166`

#### Obtener Cotización de Bono
```
GET /api/bonos/cotizacion/{mercado}/{simbolo}
```

**Parámetros:**
- `mercado` (string): Nombre del mercado (ej: "argentina")
- `simbolo` (string): Símbolo del bono (ej: "AL30")

**Respuesta (200 OK):**
```json
{
  "ultimoPrecio": 83000.0,
  "variacion": -3.37,
  "apertura": 86500.0,
  "maximo": 86900.0,
  "minimo": 83120.0,
  "fechaHora": "2025-10-23T17:00:12.2329137-03:00",
  "tendencia": "sube",
  "cierreAnterior": 85900.0,
  "montoOperado": 327469975408.008,
  "volumenNominal": 383814279.0,
  "precioPromedio": 0.0,
  "moneda": "peso_Argentino",
  "precioAjuste": 0.0,
  "interesesAbiertos": 0.0,
  "puntas": [...],
  "cantidadOperaciones": 179473,
  "descripcionTitulo": null,
  "plazo": null,
  "laminaMinima": 0,
  "lote": 0
}
```

**Errores:**
- `404 Not Found`: No se encontró el bono o error de autenticación
- `500 Internal Server Error`: Error del servidor

---

## 🎨 Frontend

### Página Bonos (/bonos)

**URL:** `http://localhost:5253/bonos`

**Campos:**
- Mercado (input text): Por defecto "argentina"
- Símbolo (input text): Por defecto "AL30"

**Botón:** "Consultar"

**Resultado:**
- Muestra todos los datos de la cotización
- Spinner mientras carga
- Mensajes de error en caso de fallo

---

## 🧪 Cómo Probar

### Opción 1: Script Automático
```bash
cd /home/asc/workspace/CryptoPriceSolution
./test_bonos.sh
```

Esto iniciará ambos servicios y probará el endpoint automáticamente.

### Opción 2: Manual

**Terminal 1 - Backend:**
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceBackend
dotnet run --urls "http://localhost:5166"
```

**Terminal 2 - Frontend:**
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm
dotnet run --urls "http://localhost:5253"
```

**Navegador:**
```
http://localhost:5253/bonos
```

**cURL:**
```bash
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30
```

---

## 📊 Ejemplos de Bonos Argentinos

| Símbolo | Descripción |
|---------|-------------|
| AL30    | Bono soberano argentino (Dollar Linked) |
| AL29    | Bono soberano argentino |
| GD30    | Bono soberano argentino (Global) |
| AE38    | Bono soberano argentino |
| AL35    | Bono soberano argentino |

**Mercado:** `argentina`

---

## ✅ Checklist de Requerimientos

- [x] Backend mantiene estructura de Crypto (Providers, Services, Controllers)
- [x] No se modificó código existente de Crypto
- [x] Frontend en Bonos.razor
- [x] Backend maneja endpoints (frontend NO llama API externa)
- [x] Usa puertos existentes (5166 backend, 5253 frontend)
- [x] No se crearon nuevos puertos
- [x] Gestión automática de tokens OAuth2
- [x] Credenciales en appsettings.json
- [x] CORS configurado correctamente
- [x] Models/DTOs en ambos proyectos
- [x] Manejo de errores

---

## 🔧 Configuración de Puertos

**Backend:**
- Puerto: 5166
- Configurado en: Properties/launchSettings.json
- URL: http://localhost:5166

**Frontend:**
- Puerto: 5253
- Configurado en: Properties/launchSettings.json
- URL: http://localhost:5253

**HttpClient (Frontend):**
```csharp
builder.Services.AddScoped(sp => 
    new HttpClient { BaseAddress = new Uri("http://localhost:5166/") });
```

---

## 📝 Notas Importantes

1. **Token expira cada 15-20 minutos**: El sistema lo renueva automáticamente
2. **Credenciales en appsettings.json**: En producción usar Azure Key Vault
3. **CORS habilitado**: Permite llamadas del frontend (puerto 5253) al backend (puerto 5166)
4. **No se toca bonos_token.json**: Ya no es necesario, el sistema gestiona tokens automáticamente
5. **Logs en consola**: Se pueden ver los requests y tokens en la consola del backend

---

## 🐛 Troubleshooting

### Error: "Authorization has been denied"
**Causa:** Token expirado o credenciales incorrectas
**Solución:** Verificar credenciales en appsettings.json, el sistema renovará el token automáticamente

### Error: Frontend no puede conectar al backend
**Causa:** Backend no está corriendo o CORS mal configurado
**Solución:** Verificar que backend esté en puerto 5166 y CORS esté habilitado

### Error: "No se encontró información para el bono"
**Causa:** Símbolo o mercado incorrecto
**Solución:** Verificar símbolo (ej: AL30) y mercado (ej: argentina)

---

## 📚 Documentación API InvertirOnline

**Token Endpoint:**
```
POST https://api.invertironline.com/token
Content-Type: application/x-www-form-urlencoded

username=EMAIL&password=PASSWORD&grant_type=password
```

**Cotización Endpoint:**
```
GET https://api.invertironline.com/api/{Mercado}/Titulos/{Simbolo}/Cotizacion
Headers:
  Authorization: Bearer {token}
  Accept: application/json
```

---

## ✨ Características Implementadas

1. ✅ Gestión automática de tokens OAuth2
2. ✅ Cache de tokens con expiración
3. ✅ Renovación automática antes de expiración
4. ✅ Arquitectura limpia (Provider → Service → Controller)
5. ✅ Frontend consume solo backend (nunca API externa)
6. ✅ Manejo de errores robusto
7. ✅ Logging detallado
8. ✅ Configuración centralizada
9. ✅ CORS configurado
10. ✅ Misma estructura que módulo Crypto
