# CryptoPriceSolution
Precios Crypto y Bonos en tiempo real.

## 🔧 Configuración

### Variables de Entorno

La aplicación utiliza variables de entorno para proteger credenciales sensibles. **Nunca versiones archivos con credenciales reales.**

#### Configuración Local (Desarrollo)

1. Copia el archivo de ejemplo:
   ```bash
   cd CryptoPriceBackend
   cp .env.example .env
   ```

2. Edita `.env` con tus credenciales reales:
   ```bash
   INVERTIRONLINE_BASE_URL=https://api.invertironline.com
   INVERTIRONLINE_USERNAME=tu_email@example.com
   INVERTIRONLINE_PASSWORD=tu_contraseña
   INVERTIRONLINE_GRANT_TYPE=password
   ```

3. El archivo `.env` está incluido en `.gitignore` y no se versionará.

#### Variables Requeridas

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `INVERTIRONLINE_BASE_URL` | URL base del API de InvertirOnline | `https://api.invertironline.com` |
| `INVERTIRONLINE_USERNAME` | Email de tu cuenta | `usuario@example.com` |
| `INVERTIRONLINE_PASSWORD` | Contraseña de tu cuenta | `MiPassword123` |
| `INVERTIRONLINE_GRANT_TYPE` | Tipo de autenticación OAuth2 | `password` |

#### Configuración en Producción

En entornos productivos, configura las variables de entorno directamente en el sistema:

**Linux/macOS:**
```bash
export INVERTIRONLINE_USERNAME="tu_email@example.com"
export INVERTIRONLINE_PASSWORD="tu_contraseña"
```

**Windows (PowerShell):**
```powershell
$env:INVERTIRONLINE_USERNAME="tu_email@example.com"
$env:INVERTIRONLINE_PASSWORD="tu_contraseña"
```

**Docker:**
```yaml
environment:
  - INVERTIRONLINE_USERNAME=tu_email@example.com
  - INVERTIRONLINE_PASSWORD=tu_contraseña
```

### Orden de Prioridad de Configuración

1. **Variables de entorno** (recomendado para credenciales)
2. **appsettings.json** (solo para valores no sensibles)

## 🚀 Ejecución

```bash
# Backend
cd CryptoPriceBackend
dotnet run

# Frontend
cd CryptoPriceFrontendWasm
dotnet run
```

## ⚠️ Seguridad

- ✅ Los archivos `.env` están en `.gitignore`
- ✅ Los archivos `appsettings.*.local.json` están en `.gitignore`
- ✅ El archivo `bonos_token.json` está en `.gitignore`
- ❌ **NUNCA** comitees archivos con credenciales reales
- ❌ **NUNCA** expongas tus credenciales en repositorios públicos
