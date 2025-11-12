# 🚨 ISSUE: API de InvertirOnline - Serie Histórica

## Problema Detectado

La API de InvertirOnline está devolviendo **400 Bad Request** al intentar obtener series históricas de bonos.

## Endpoint Problemático

```
GET /api/bCBA/Titulos/{simbolo}/Cotizacion/seriehistorica/{fechaDesde}/{fechaHasta}/{ajustada}
```

## URL Generada

```
https://api.invertironline.com/api/bCBA/Titulos/AL30/Cotizacion/seriehistorica/25%20Sep%202025%2023%3A17%3A23%20GMT/25%20Oct%202025%2023%3A17%3A23%20GMT/ajustada?api_key={token}
```

## Formato de Fecha Probado

```
25 Sep 2025 23:17:23 GMT
```

Basado en el ejemplo proporcionado:
```
10 Oct 2025 22:15:51 GMT
```

## Respuesta del Servidor

```html
<!DOCTYPE html>
<html>
    <head>
        <title>Runtime Error</title>
...
    <body bgcolor="white">
        <h1>Server Error in '/' Application.</h1>
        <h2>Runtime Error</h2>
        <b>Description:</b> An application error occurred on the server...
```

## Posibles Causas

1. **Formato de fecha incorrecto**
   - La API podría requerir un formato específico no documentado
   - Podría necesitar nombres de meses en español
   - Podría requerir un formato ISO 8601

2. **Parámetros faltantes**
   - Podrían faltar query parameters adicionales
   - El mercado `bCBA` podría no ser válido para series históricas

3. **Permisos de API**
   - Las credenciales podrían no tener acceso a series históricas
   - Podría requerir un nivel de suscripción diferente

4. **Problema del servidor**
   - Error interno de la API de InvertirOnline
   - Endpoint temporalmente fuera de servicio

## Soluciones Propuestas

### Solución 1: Usar API alternativa (Recomendado)

Usar la API pública de Yahoo Finance o Alpha Vantage para obtener datos históricos:

```csharp
// Yahoo Finance (gratuita)
https://query1.finance.yahoo.com/v8/finance/chart/{symbol}?interval=1d&range=1mo

// Alpha Vantage (API key gratuita)
https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={key}
```

### Solución 2: Simular datos (Para demo)

Crear un endpoint de prueba que devuelva datos simulados para demostrar la funcionalidad:

```csharp
// En BonosProvider.cs
private BonosSerieHistoricaResponse GenerarDatosSimulados(
    string simbolo, 
    DateTime desde, 
    DateTime hasta)
{
    var datos = new List<BonoHistoricoDataPoint>();
    var precioBase = 80000 + (simbolo.GetHashCode() % 20000);
    var fecha = desde;
    
    while (fecha <= hasta)
    {
        var variacion = (Random.Shared.NextDouble() * 10) - 5; // -5% a +5%
        var precio = precioBase * (1 + variacion / 100);
        
        datos.Add(new BonoHistoricoDataPoint
        {
            Fecha = fecha,
            Precio = precio,
            Variacion = variacion,
            Apertura = precio * 0.99,
            Maximo = precio * 1.02,
            Minimo = precio * 0.98,
            Volumen = Random.Shared.Next(100000, 500000)
        });
        
        fecha = fecha.AddDays(1);
        precioBase = (int)precio;
    }
    
    return new BonosSerieHistoricaResponse
    {
        Simbolo = simbolo,
        Datos = datos,
        FechaDesde = desde,
        FechaHasta = hasta
    };
}
```

### Solución 3: Contactar soporte de InvertirOnline

Preguntar por:
1. Formato correcto de fechas para series históricas
2. Documentación completa del endpoint
3. Ejemplos de uso exitosos
4. Permisos requeridos en la cuenta

## Workaround Temporal

Mientras se resuelve el issue, implementar datos de prueba en el frontend:

```csharp
// En BonosCharts.razor
private async Task CargarDatosSimulados()
{
    var datos = new List<BonoHistoricoDataPoint>();
    var fecha = DateTime.Now.AddMonths(-1);
    var precioBase = 83000.0;
    
    for (int i = 0; i < 30; i++)
    {
        var variacion = (Random.Shared.NextDouble() * 6) - 3;
        var precio = precioBase + (variacion * 1000);
        
        datos.Add(new BonoHistoricoDataPoint
        {
            Fecha = fecha.AddDays(i),
            Precio = precio,
            Variacion = variacion,
            Maximo = precio * 1.01,
            Minimo = precio * 0.99,
            Volumen = Random.Shared.Next(300000, 600000)
        });
        
        precioBase = precio;
    }
    
    seriesData[simbolo] = new BonosSerieHistoricaResponse
    {
        Simbolo = simbolo,
        Datos = datos
    };
}
```

## Estado Actual

- ✅ Frontend completamente implementado
- ✅ Backend con arquitectura correcta
- ✅ Endpoint de cotización simple funcionando
- ❌ Endpoint de serie histórica con error 400
- ⏳ Pendiente: Resolver formato de fecha o usar API alternativa

## Próximos Pasos

1. **Inmediato**: Implementar datos simulados para demo
2. **Corto plazo**: Contactar soporte de InvertirOnline
3. **Alternativa**: Migrar a API de Yahoo Finance o Alpha Vantage

---

**Fecha**: 2025-10-25
**Status**: 🔴 Bloqueado por API externa
