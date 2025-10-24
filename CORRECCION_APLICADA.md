# 🔧 Corrección Aplicada - Módulo de Bonos

## Problema Identificado

El backend estaba recibiendo **HTTP 400 (Bad Request)** de la API de InvertirOnline porque la URL no incluía los parámetros query necesarios.

## Análisis del Error

### Error Original:
```
[BonosProvider] Llamando a: https://api.invertironline.com/api/argentina/Titulos/AL30/Cotizacion
[BonosProvider] HTTP 400 - Response: ...
[BonosProvider] Error en la respuesta: BadRequest
```

### Curl que funcionaba:
```bash
curl 'https://api.invertironline.com/api/argentina/Titulos/AL30/Cotizacion?mercado=argentina&simbolo=AL30&model.simbolo=AL30&model.mercado=bCBA&model.plazo=t0&api_key=TOKEN'
```

**Diferencia clave:** El curl incluía query parameters (`?mercado=...&simbolo=...&model.*`), pero el código no.

---

## Solución Aplicada

### Archivo modificado: `BonosProvider.cs`

**Antes:**
```csharp
var url = $"https://api.invertironline.com/api/{mercado}/Titulos/{simbolo}/Cotizacion";
```

**Después:**
```csharp
var simboloUpper = simbolo.ToUpper();
var simboloLower = simbolo.ToLower();

var url = $"https://api.invertironline.com/api/{mercado}/Titulos/{simboloUpper}/Cotizacion" +
          $"?mercado={mercado}" +
          $"&simbolo={simboloUpper}" +
          $"&model.simbolo={simboloUpper}" +
          $"&model.mercado=bCBA" +
          $"&model.plazo=t0";
```

### Cambios realizados:

1. ✅ **Query Parameters**: Agregados `mercado`, `simbolo`, `model.simbolo`, `model.mercado`, `model.plazo`
2. ✅ **Case Handling**: Convertir símbol a mayúsculas (`AL30` en vez de `al30`)
3. ✅ **Logging mejorado**: Agregado log de respuesta completa en caso de error
4. ✅ **Formato de URL**: Coincide exactamente con el curl que funciona

---

## Resultado

### ✅ Prueba Exitosa

```bash
$ curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30
```

**Respuesta HTTP 200 OK:**
```json
{
    "ultimoPrecio": 83000,
    "variacion": 0,
    "apertura": 86500,
    "maximo": 86900,
    "minimo": 83120,
    "fechaHora": "2025-10-24T04:01:01.1107653+00:00",
    "tendencia": "sube",
    "cierreAnterior": 83000,
    "montoOperado": 0,
    "volumenNominal": 0,
    "precioPromedio": 0,
    "moneda": "peso_Argentino",
    "precioAjuste": 0,
    "interesesAbiertos": 0,
    "puntas": [],
    "cantidadOperaciones": 0,
    "descripcionTitulo": null,
    "plazo": null,
    "laminaMinima": 0,
    "lote": 0
}
```

✅ **El backend ahora funciona correctamente!**

---

## Próximos Pasos

### 1. Reiniciar Frontend

El frontend necesita reiniciarse para que tome los cambios en `Program.cs`:

```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm
dotnet run
```

### 2. Probar en Navegador

Abrir: http://localhost:5253/bonos

### 3. Consultar Bono

- Mercado: `argentina`
- Símbolo: `AL30`
- Click "Consultar"

**Resultado esperado:** Muestra la cotización correctamente

---

## Explicación Técnica

### ¿Por qué el 400?

La API de InvertirOnline requiere **query parameters específicos** además del path. Estos parámetros son parte del modelo de request:

```
/api/{Mercado}/Titulos/{Simbolo}/Cotizacion
  ?mercado={mercado}           # Duplicado del path, requerido
  &simbolo={simbolo}           # Duplicado del path, requerido  
  &model.simbolo={simbolo}     # Parte del modelo
  &model.mercado=bCBA          # Mercado de operación
  &model.plazo=t0              # Plazo de liquidación
```

Sin estos parámetros, la API devuelve **400 Bad Request**.

### Parámetros del Modelo

- **model.mercado=bCBA**: Mercado de Buenos Aires (Bolsa de Comercio de Buenos Aires)
- **model.plazo=t0**: Liquidación inmediata (T+0)

Estos son valores estándar para bonos argentinos.

---

## Estado Actual

| Componente | Estado | Notas |
|------------|--------|-------|
| **Backend** | ✅ Funcionando | Responde HTTP 200 con datos |
| **Token OAuth2** | ✅ Funcionando | Se obtiene y cachea correctamente |
| **API Externa** | ✅ Funcionando | Responde con cotización |
| **Endpoint** | ✅ Funcionando | `/api/bonos/cotizacion/{mercado}/{simbolo}` |
| **Frontend** | ⏳ Pendiente reinicio | Necesita reiniciarse |

---

## Comandos Útiles

### Verificar Backend
```bash
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30 | python3 -m json.tool
```

### Iniciar Frontend
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm
dotnet run
```

### Ver Logs Backend
```bash
# Si usaste ./run.sh, los logs están en:
tail -f backend.log

# Si lo iniciaste manualmente, los logs están en la consola
```

### Detener Servicios
```bash
pkill -9 dotnet
```

---

## Troubleshooting

### Si sigue dando error 400

1. **Verificar credenciales**: `appsettings.json` debe tener username/password correctos
2. **Token expirado**: El sistema renueva automáticamente, espera 1 minuto
3. **Símbolo incorrecto**: Usar símbolos válidos: AL30, AL29, GD30, etc.

### Si da error 404 desde frontend

1. **Backend no corriendo**: Verificar con `lsof -i :5166`
2. **Frontend BaseAddress incorrecto**: Debe ser `http://localhost:5166/`
3. **CORS**: Verificar que CORS esté habilitado en backend

### Si da error de puerto ocupado

```bash
# Matar procesos en puerto 5166 o 5253
lsof -ti :5166 | xargs kill -9
lsof -ti :5253 | xargs kill -9
```

---

## Cambios en Código

### Archivo: `CryptoPriceBackend/Providers/BonosProvider.cs`

**Líneas 107-122** (aprox):
```csharp
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
```

**Líneas 133-136** (aprox):
```csharp
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"[BonosProvider] Error en la respuesta: {response.StatusCode}");
    Console.WriteLine($"[BonosProvider] Response completa: {raw}");  // <-- Línea nueva
    return null;
}
```

---

## ✅ Corrección Completada

El módulo de Bonos ahora **funciona correctamente**:

1. ✅ Backend obtiene token OAuth2 automáticamente
2. ✅ Backend llama a API externa con parámetros correctos
3. ✅ Backend retorna HTTP 200 con cotización
4. ⏳ Frontend pendiente de reinicio

**Próximo paso:** Reiniciar el frontend y probar en navegador.

---

**Fecha de corrección:** 24 de Octubre de 2025, 04:30 GMT
**Estado:** ✅ BACKEND FUNCIONANDO - FRONTEND PENDIENTE REINICIO
