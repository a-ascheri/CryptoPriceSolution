# Módulo de Bonos - CryptoPriceSolution

## Descripción

Se ha implementado un módulo completo de Bonos siguiendo la misma arquitectura que el módulo de precios de criptomonedas, sin modificar ningún código existente.

## Arquitectura Backend

### Providers
- **IBonosProvider**: Interfaz que define el contrato para obtener cotizaciones
- **BonosProvider**: Implementación que:
  - Gestiona automáticamente la autenticación con InvertirOnline
  - Obtiene y cachea tokens de acceso
  - Renueva el token automáticamente antes de su expiración
  - Realiza las llamadas HTTP autenticadas a la API de InvertirOnline

### Services
- **IBonosService**: Interfaz del servicio de negocio
- **BonosService**: Implementación que actúa como capa intermedia entre Controller y Provider

### Controllers
- **BonosController**: Expone el endpoint REST:
  - `GET /api/bonos/cotizacion/{mercado}/{simbolo}`
  - Ejemplo: `http://localhost:5166/api/bonos/cotizacion/argentina/AL30`

### Models
- **BonosCotizacionResponse**: DTO que mapea la respuesta de la API de InvertirOnline
- **Punta**: Clase auxiliar para las puntas de compra/venta

## Arquitectura Frontend

### Pages
- **Bonos.razor**: Página Blazor que:
  - Permite ingresar mercado y símbolo
  - Consulta el backend (no la API externa)
  - Muestra la cotización con todos los detalles

### Models
- **BonosCotizacionResponse**: Modelo duplicado en el frontend para deserializar la respuesta

## Configuración

### Backend (appsettings.json)
```json
{
  "InvertirOnline": {
    "BaseUrl": "https://api.invertironline.com",
    "Username": "tu_usuario@email.com",
    "Password": "tu_contraseña",
    "GrantType": "password"
  }
}
```

### Puertos
- **Backend**: 5166 (no modificado)
- **Frontend**: 5253 (no modificado)

## Cómo probar

### 1. Verificar credenciales
Asegúrate de que las credenciales en `appsettings.json` sean correctas.

### 2. Iniciar el backend
```bash
cd CryptoPriceBackend
dotnet run
```

El backend estará disponible en: http://localhost:5166

### 3. Iniciar el frontend
```bash
cd CryptoPriceFrontendWasm
dotnet run
```

El frontend estará disponible en: http://localhost:5253

### 4. Probar desde el navegador
- Abre: http://localhost:5253/bonos
- Ingresa:
  - Mercado: `argentina`
  - Símbolo: `AL30`
- Haz clic en "Consultar"

### 5. Probar el endpoint del backend directamente
```bash
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30
```

## Ejemplos de bonos argentinos

- **AL30**: Bono soberano argentino
- **AL29**: Bono soberano argentino
- **GD30**: Bono soberano argentino
- **AE38**: Bono soberano argentino

Mercado: `argentina`

## Estructura de la respuesta

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
  "moneda": "peso_Argentino",
  "puntas": [...],
  "cantidadOperaciones": 179473
}
```

## Notas técnicas

### Gestión de tokens
- El token se obtiene automáticamente al hacer la primera llamada
- Se cachea en memoria
- Se renueva automáticamente 1 minuto antes de expirar
- No es necesario gestionar manualmente el archivo `bonos_token.json`

### Seguridad
- Las credenciales están en `appsettings.json` (en producción usar Azure Key Vault o variables de entorno)
- El token nunca se expone al frontend
- Toda la autenticación se maneja en el backend

### CORS
- Configurado en el backend para permitir llamadas desde el frontend
- En desarrollo: permite cualquier origen
- En producción: configurar dominios específicos

## Troubleshooting

### Error: "Authorization has been denied for this request"
- Verifica que las credenciales en `appsettings.json` sean correctas
- El token puede haber expirado, el sistema lo renovará automáticamente en el siguiente request

### Error de conexión desde el frontend
- Verifica que el backend esté corriendo en el puerto 5166
- Verifica que CORS esté habilitado correctamente
- Abre la consola del navegador para ver errores detallados

### El backend no inicia
- Verifica que .NET 8 SDK esté instalado: `dotnet --version`
- Restaura paquetes: `dotnet restore`
- Limpia y rebuild: `dotnet clean && dotnet build`
