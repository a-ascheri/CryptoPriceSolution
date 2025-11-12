# 🚀 Guía Rápida - Bonos Charts

## Inicio Rápido (5 minutos)

### Opción 1: Script Automático (Recomendado)

```bash
cd /home/asc/workspace/CryptoPriceSolution
./start_bonos_charts.sh
```

Este script:
- ✅ Verifica y libera puertos
- ✅ Compila backend y frontend
- ✅ Inicia ambos servicios
- ✅ Abre el navegador automáticamente
- ✅ Muestra logs en tiempo real

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
http://localhost:5253/bonos-charts
```

---

## 📊 Cómo Usar la Aplicación

### Paso 1: Acceder
Abre tu navegador y ve a: **http://localhost:5253/bonos-charts**

### Paso 2: Seleccionar Bonos
1. **País**: Selecciona "Argentina" (ya viene por defecto)
2. **Bonos**: Click en el dropdown "Bonos a Comparar"
   - Puedes seleccionar múltiples bonos
   - Ejemplo: AL30, AL29, GD30
3. **Período**: Selecciona el rango temporal
   - 1M = 1 Mes
   - 3M = 3 Meses
   - 6M = 6 Meses
   - 1A = 1 Año
   - 5A = 5 Años
   - MAX = Máximo disponible

### Paso 3: Ver Resultados
Click en **"Actualizar"** y verás:

#### 📈 Gráfico de Evolución de Precios
- Líneas de tiempo comparando precios de los bonos seleccionados
- Pasa el mouse sobre los puntos para ver valores exactos
- Zoom con la rueda del mouse
- Pan arrastrando con el mouse

#### 📊 Gráfico de Variación Porcentual
- Barras mostrando la variación de cada período
- Comparación visual entre bonos

#### 📋 Tabla de Métricas Clave
- Precio Actual, Máximo, Mínimo, Promedio
- Volatilidad calculada
- Variación del período (con indicadores ↑↓)

#### 📝 Datos Históricos Detallados
- Tabla paginada con todos los datos
- Ordenable por cualquier columna
- 10 registros por página

---

## 🎯 Casos de Uso Comunes

### Comparar dos bonos argentinos
```
1. País: Argentina
2. Bonos: AL30, AL29
3. Período: 1M
4. Click "Actualizar"
```

### Ver evolución de largo plazo
```
1. País: Argentina
2. Bonos: GD30
3. Período: 5A
4. Click "Actualizar"
```

### Análisis de volatilidad
```
1. País: Argentina
2. Bonos: AL30, AL29, GD30, GD35
3. Período: 6M
4. Click "Actualizar"
5. Ver columna "Volatilidad" en tabla de métricas
```

---

## 📋 Bonos Disponibles

### Argentina
| Símbolo | Nombre Completo |
|---------|-----------------|
| **AL30** | Bono Argentina USD 2030 |
| **AL29** | Bono Argentina USD 2029 |
| **GD30** | Global 2030 |
| **AE38** | Bono Argentina EUR 2038 |
| **AL35** | Bono Argentina USD 2035 |
| **GD35** | Global 2035 |
| **GD38** | Global 2038 |
| **GD41** | Global 2041 |
| **GD46** | Global 2046 |

---

## 🔧 Solución de Problemas

### Problema: "No se pudieron obtener datos"

**Solución 1**: Verificar backend
```bash
# Verificar que el backend esté corriendo
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30

# Debería devolver JSON con cotización
```

**Solución 2**: Ver logs del backend
```bash
tail -f /home/asc/workspace/CryptoPriceSolution/logs/backend.log
```

**Solución 3**: Verificar credenciales
Editar `/home/asc/workspace/CryptoPriceSolution/CryptoPriceBackend/appsettings.json`

### Problema: Gráficos no se muestran

**Solución 1**: Limpiar cache del navegador
- Presiona `Ctrl + Shift + R` (forzar recarga)
- O `Ctrl + F5`

**Solución 2**: Verificar consola del navegador
- Presiona `F12`
- Ve a la pestaña "Console"
- Busca errores en rojo

**Solución 3**: Recompilar frontend
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm
dotnet clean
dotnet build
dotnet run --urls "http://localhost:5253"
```

### Problema: Puerto ocupado

**Solución**: Liberar puerto
```bash
# Para puerto 5166 (backend)
lsof -ti:5166 | xargs kill -9

# Para puerto 5253 (frontend)
lsof -ti:5253 | xargs kill -9
```

---

## 📊 Endpoints de la API

### Cotización Actual
```http
GET http://localhost:5166/api/bonos/cotizacion/{mercado}/{simbolo}
```

**Ejemplo**:
```bash
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30
```

### Serie Histórica
```http
GET http://localhost:5166/api/bonos/historico/{mercado}/{simbolo}?rangoTemporal={rango}
```

**Ejemplos**:
```bash
# Último mes
curl http://localhost:5166/api/bonos/historico/argentina/AL30?rangoTemporal=1M

# Últimos 3 meses
curl http://localhost:5166/api/bonos/historico/argentina/AL30?rangoTemporal=3M

# Con fechas personalizadas
curl "http://localhost:5166/api/bonos/historico/argentina/AL30?fechaDesde=2025-01-01&fechaHasta=2025-10-25"
```

---

## 🎨 Personalización

### Cambiar Tema de Radzen

Editar `/home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm/wwwroot/index.html`:

**Material Theme (Actual)**:
```html
<link rel="stylesheet" href="_content/Radzen.Blazor/css/material-base.css">
<link rel="stylesheet" href="_content/Radzen.Blazor/css/material.css">
```

**Dark Theme**:
```html
<link rel="stylesheet" href="_content/Radzen.Blazor/css/dark-base.css">
<link rel="stylesheet" href="_content/Radzen.Blazor/css/dark.css">
```

**Standard Theme**:
```html
<link rel="stylesheet" href="_content/Radzen.Blazor/css/standard-base.css">
<link rel="stylesheet" href="_content/Radzen.Blazor/css/standard.css">
```

### Agregar Más Bonos

Editar `/home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm/Services/BondDataService.cs`:

```csharp
public static List<BonoInfo> BonosArgentinos = new()
{
    // ... bonos existentes ...
    
    // Agregar nuevo bono
    new BonoInfo 
    { 
        Simbolo = "NUEVO", 
        Nombre = "Nuevo Bono Argentina", 
        Pais = "Argentina", 
        Mercado = "argentina" 
    },
};
```

Luego recompilar:
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm
dotnet build
```

---

## 📁 Estructura de Archivos Importantes

```
CryptoPriceSolution/
├── CryptoPriceBackend/
│   ├── Controllers/
│   │   └── BonosController.cs          ← Endpoints de API
│   ├── Services/
│   │   ├── BonosService.cs             ← Lógica de negocio
│   │   └── BondCacheService.cs         ← Cache de datos
│   ├── Providers/
│   │   └── BonosProvider.cs            ← Llamadas a API externa
│   ├── Models/
│   │   ├── BonosCotizacionResponse.cs
│   │   └── BonosSerieHistoricaResponse.cs
│   └── appsettings.json                ← Credenciales
│
├── CryptoPriceFrontendWasm/
│   ├── Pages/
│   │   ├── Bonos.razor                 ← Página simple
│   │   └── BonosCharts.razor           ← Página con gráficos ★
│   ├── Services/
│   │   └── BondDataService.cs          ← Servicio de datos
│   ├── Models/
│   │   └── BonosSerieHistoricaResponse.cs
│   └── wwwroot/
│       └── index.html                  ← Incluye Radzen CSS/JS
│
├── start_bonos_charts.sh               ← Script de inicio
└── BONOS_CHARTS_IMPLEMENTATION.md      ← Documentación completa
```

---

## 💡 Tips y Trucos

### 1. Acelerar el Inicio
```bash
# Compilar en Release mode
cd CryptoPriceBackend
dotnet build -c Release

cd ../CryptoPriceFrontendWasm
dotnet build -c Release
```

### 2. Ver Logs en Tiempo Real
```bash
# Backend
tail -f /home/asc/workspace/CryptoPriceSolution/logs/backend.log

# Frontend
tail -f /home/asc/workspace/CryptoPriceSolution/logs/frontend.log
```

### 3. Verificar Estado de Servicios
```bash
# Ver procesos corriendo
ps aux | grep dotnet

# Ver puertos abiertos
netstat -tuln | grep -E '5166|5253'
```

### 4. Detener Servicios
```bash
# Si usaste el script
./close.sh

# O manualmente
kill $(cat .backend.pid)
kill $(cat .frontend.pid)
```

---

## 📚 Más Información

- **Documentación Completa**: `BONOS_CHARTS_IMPLEMENTATION.md`
- **Guía Técnica de Bonos**: `BONOS_TECHNICAL_SUMMARY.md`
- **README Principal**: `BONOS_README.md`

---

## 🆘 Ayuda

### Obtener Ayuda
```bash
# Ver ayuda del script
./start_bonos_charts.sh --help

# Ver versiones
dotnet --version
```

### Reportar Problemas
Si encuentras algún problema:
1. Verifica los logs (backend y frontend)
2. Revisa la consola del navegador (F12)
3. Asegúrate de tener .NET 8.0 instalado
4. Verifica las credenciales de InvertirOnline

---

## ✅ Checklist de Verificación

Antes de empezar, asegúrate de tener:

- [ ] .NET 8.0 SDK instalado
- [ ] Puerto 5166 disponible (backend)
- [ ] Puerto 5253 disponible (frontend)
- [ ] Credenciales de InvertirOnline en `appsettings.json`
- [ ] Conexión a internet (para API de InvertirOnline)

Para verificar:
```bash
# Verificar .NET
dotnet --version

# Verificar puertos
lsof -i :5166
lsof -i :5253

# Si hay procesos, liberarlos
lsof -ti:5166 | xargs kill -9
lsof -ti:5253 | xargs kill -9
```

---

¡Listo para analizar bonos! 🚀📊
