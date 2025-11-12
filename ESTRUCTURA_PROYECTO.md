# 📁 Estructura del Proyecto CryptoPriceSolution

## ✅ Estructura Limpia y Organizada

```
CryptoPriceSolution/
│
├── CryptoPriceSolution.sln           # Solución principal
│
├── CryptoPriceBackend/                # 🔧 Backend unificado (Puerto 5166)
│   ├── Program.cs                     # Configuración y servicios
│   ├── appsettings.json              # Configuración
│   ├── bonos_token.json              # Token de InvertirOnline
│   │
│   ├── Controllers/                   # 🎮 Controladores REST API
│   │   ├── BonosController.cs        # Endpoints de bonos
│   │   └── PricesController.cs       # Endpoints de criptomonedas
│   │
│   ├── Models/                        # 📊 Modelos de datos
│   │   ├── BonosCotizacionResponse.cs
│   │   └── BonosSerieHistoricaResponse.cs
│   │
│   ├── Providers/                     # 🔌 Integraciones con APIs externas
│   │   ├── IBonosProvider.cs         # Interface para bonos
│   │   ├── BonosProvider.cs          # Implementación InvertirOnline
│   │   ├── ICurrencyProvider.cs      # Interface para crypto
│   │   ├── CoinGeckoProvider.cs      # Implementación CoinGecko
│   │   └── ExchangeRateProvider.cs   # Implementación tasas de cambio
│   │
│   └── Services/                      # 🛠️ Lógica de negocio
│       ├── IBonosService.cs          # Interface de servicio de bonos
│       ├── BonosService.cs           # Lógica de negocio bonos
│       ├── BondCacheService.cs       # Cache de bonos
│       ├── IPriceService.cs          # Interface de servicio de precios
│       └── PriceService.cs           # Lógica de negocio precios
│
└── CryptoPriceFrontendWasm/           # 🎨 Frontend Blazor WebAssembly (Puerto 5253)
    ├── Program.cs                     # Punto de entrada
    ├── App.razor                      # Componente raíz
    ├── _Imports.razor                 # Imports globales
    │
    ├── Layout/                        # 📐 Componentes de layout
    │   ├── MainLayout.razor          # Layout principal
    │   └── NavMenu.razor             # Menú de navegación
    │
    ├── Pages/                         # 📄 Páginas de la aplicación
    │   ├── Home.razor                # Página principal
    │   ├── PriceChecker.razor        # Checker de criptomonedas
    │   ├── Bonos.razor               # Tabla de bonos
    │   ├── BonosCharts.razor         # 📊 Gráficos históricos bonos
    │   └── Letras.razor              # Tabla de letras
    │
    ├── Models/                        # 📊 Modelos del frontend
    │   ├── BonosCotizacionResponse.cs
    │   └── BonosSerieHistoricaResponse.cs
    │
    ├── Services/                      # 🔧 Servicios del frontend
    │   ├── BonosService.cs           # Comunicación con backend (bonos)
    │   └── BondDataService.cs        # Catálogo y datos de bonos
    │
    └── wwwroot/                       # 🌐 Archivos estáticos
        ├── index.html                 # HTML principal
        ├── css/                       # Estilos
        └── sample-data/              # Datos de ejemplo
```

---

## 🎯 Arquitectura del Backend

### Patrón: Provider → Service → Controller

```
Cliente HTTP Request
        ↓
[BonosController]           ← Expone REST API
        ↓
[BonosService]              ← Lógica de negocio + Cache
        ↓
[BondCacheService]          ← Cache en memoria (5 min TTL)
        ↓
[BonosProvider]             ← Comunicación con InvertirOnline API
        ↓
API Externa (InvertirOnline)
```

### Responsabilidades

**Controllers** (`BonosController.cs`)
- ✅ Recibir requests HTTP
- ✅ Validar parámetros
- ✅ Mapear parámetros de entrada
- ✅ Retornar respuestas HTTP

**Services** (`BonosService.cs`)
- ✅ Lógica de negocio
- ✅ Orquestación de providers
- ✅ Integración con cache
- ✅ Transformación de datos

**Providers** (`BonosProvider.cs`)
- ✅ Comunicación con APIs externas
- ✅ Manejo de autenticación
- ✅ Serialización/deserialización
- ✅ Manejo de errores de API
- ✅ Generación de datos simulados (fallback)

**Cache** (`BondCacheService.cs`)
- ✅ Almacenamiento en memoria
- ✅ Expiración automática (TTL)
- ✅ Thread-safe (ConcurrentDictionary)

---

## 🎨 Arquitectura del Frontend

### Patrón: Page → Service → Backend API

```
[BonosCharts.razor]         ← Componente de UI
        ↓
[BondDataService.cs]        ← Catálogo + Comunicación HTTP
        ↓
[HttpClient]                ← Cliente HTTP
        ↓
Backend API (puerto 5166)
```

### Responsabilidades

**Pages** (`BonosCharts.razor`)
- ✅ Renderizado de UI
- ✅ Manejo de eventos de usuario
- ✅ Estado local del componente
- ✅ Visualización de gráficos (Radzen)

**Services** (`BondDataService.cs`)
- ✅ Comunicación con backend
- ✅ Catálogo de bonos disponibles
- ✅ Transformación de datos para UI
- ✅ Manejo de errores HTTP

**Models** (`BonosSerieHistoricaResponse.cs`)
- ✅ DTOs para deserialización JSON
- ✅ Estructuras de datos tipadas

---

## 🔌 APIs y Endpoints

### Backend Endpoints

#### 1. **Cotización de Bonos**
```http
GET /api/bonos/cotizacion/{mercado}
```
- **Parámetros**: `mercado` (argentina, eeuu, brasil)
- **Respuesta**: Lista de bonos con precio, variación, volumen

#### 2. **Serie Histórica**
```http
GET /api/bonos/historico/{mercado}/{simbolo}
```
- **Parámetros**: 
  - `mercado`: argentina, eeuu, brasil
  - `simbolo`: AL30, AL29, GD30, etc.
  - `rangoTemporal` (query): 1M, 3M, 6M, 1A, 5A, MAX
  - `fechaDesde` (query): Fecha inicio (opcional)
  - `fechaHasta` (query): Fecha fin (opcional)
  - `ajustada` (query): true/false
- **Respuesta**: Objeto con datos históricos (array de puntos)

#### 3. **Precios de Criptomonedas**
```http
GET /api/prices/{cryptoId}
```
- **Parámetros**: `cryptoId` (bitcoin, ethereum, etc.)
- **Respuesta**: Precio en USD y otras divisas

---

## 🚀 Puertos

| Servicio | Puerto | URL |
|----------|--------|-----|
| **Backend** | 5166 | http://localhost:5166 |
| **Frontend** | 5253 | http://localhost:5253 |

---

## 📦 Dependencias Principales

### Backend (CryptoPriceBackend)
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" />
<PackageReference Include="Swashbuckle.AspNetCore" />
```

### Frontend (CryptoPriceFrontendWasm)
```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" />
<PackageReference Include="Radzen.Blazor" Version="5.6.9" />
```

---

## 🎯 Características Implementadas

### ✅ Módulo de Criptomonedas
- Consulta de precios en tiempo real (CoinGecko)
- Conversión a múltiples divisas
- Cache de respuestas

### ✅ Módulo de Bonos - Cotización
- Listado de bonos por mercado
- Precio actual, variación, volumen
- Filtrado por país

### ✅ Módulo de Bonos - Análisis Histórico
- **Gráficos interactivos** (Radzen Blazor Charts)
  - Gráfico de líneas: Evolución de precios
  - Gráfico de columnas: Variación porcentual
- **Controles de usuario**
  - Selector de país (Argentina, EEUU, Brasil)
  - Multi-selección de bonos (AL30, AL29, GD30, etc.)
  - Filtros temporales (1M, 3M, 6M, 1A, 5A, MAX)
  - Botón de actualización
- **Métricas calculadas**
  - Precio actual, máximo, mínimo, promedio
  - Volatilidad (desviación estándar)
  - Variación del período
  - Indicadores visuales (↑↓)
- **Tablas de datos**
  - Tabla de métricas clave
  - Tabla de datos históricos detallados
  - Paginación (10 registros/página)
- **Diseño responsive**
  - Bootstrap 5
  - Radzen Blazor components

---

## 🗑️ Limpieza Realizada

### ❌ Eliminado: `/home/asc/workspace/InvertirOnlineBackend`
- **Razón**: Proyecto vacío con solo código de ejemplo (WeatherForecast)
- **Estado**: TODO el código funcional ya está en `CryptoPriceBackend`

### ❌ Eliminado: `CryptoPriceSolution/InvertirOnlineFrontendWasm/`
- **Razón**: Solo contenía carpeta `obj/` con archivos temporales de compilación
- **Estado**: TODO el código funcional ya está en `CryptoPriceFrontendWasm`

---

## ✅ Estructura Final

### Proyectos en la Solución (.sln)
```
CryptoPriceSolution.sln
├── CryptoPriceBackend          (Backend unificado)
└── CryptoPriceFrontendWasm     (Frontend unificado)
```

### Todo Centralizado
- ✅ **1 Backend** con todos los endpoints (bonos + crypto)
- ✅ **1 Frontend** con todas las páginas (bonos + crypto)
- ✅ Sin duplicados ni proyectos vacíos
- ✅ Estructura clara y mantenible

---

## 🚦 Cómo Ejecutar

### Opción 1: Script Automático
```bash
cd /home/asc/workspace/CryptoPriceSolution
./start_bonos_charts.sh
```

### Opción 2: Manual

**Backend:**
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceBackend
dotnet run --urls "http://localhost:5166"
```

**Frontend:**
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm
dotnet watch run --urls "http://localhost:5253"
```

### Opción 3: Desde Visual Studio
```bash
dotnet build CryptoPriceSolution.sln
dotnet run --project CryptoPriceBackend
dotnet run --project CryptoPriceFrontendWasm
```

---

## 📚 Documentación

- `README.md` - Documentación general
- `BONOS_CHARTS_INDEX.md` - Índice del módulo de gráficos
- `BONOS_CHARTS_QUICKSTART.md` - Guía rápida de uso
- `BONOS_CHARTS_IMPLEMENTATION.md` - Documentación técnica detallada
- `API_ISSUE_SERIE_HISTORICA.md` - Problemas conocidos con API
- `SOLUCION_DATOS_SIMULADOS.md` - Workaround datos simulados
- `ESTRUCTURA_PROYECTO.md` - Este documento

---

## 🎉 Estado Actual

**✅ PROYECTO LIMPIO Y FUNCIONAL**

- [x] Estructura organizada
- [x] Sin duplicados
- [x] Backend unificado
- [x] Frontend unificado
- [x] Todos los features funcionando
- [x] Documentación completa
- [x] Scripts de inicio
- [x] Datos simulados (workaround API)

---

**Última actualización**: 2025-11-12  
**Versión**: 2.0 (Estructura limpia)
