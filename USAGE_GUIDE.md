# Guía de Uso - Módulo de Bonos

## 🚀 Inicio Rápido

### Opción 1: Script Automático (Recomendado)

```bash
cd /home/asc/workspace/CryptoPriceSolution
./test_bonos.sh
```

Este script:
- ✅ Compila backend y frontend
- ✅ Inicia ambos servicios
- ✅ Prueba el endpoint automáticamente
- ✅ Muestra URLs e instrucciones

### Opción 2: Scripts Existentes

**Iniciar todo:**
```bash
cd /home/asc/workspace/CryptoPriceSolution
./run.sh
```

**Detener todo:**
```bash
cd /home/asc/workspace/CryptoPriceSolution
./close.sh
```

### Opción 3: Manual

**Terminal 1 - Backend:**
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceBackend
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceFrontendWasm
dotnet run
```

---

## 🌐 URLs de Acceso

| Servicio | URL | Descripción |
|----------|-----|-------------|
| Frontend - Home | http://localhost:5253 | Página principal |
| Frontend - Bonos | http://localhost:5253/bonos | Módulo de Bonos |
| Frontend - Crypto | http://localhost:5253/price-checker | Módulo de Crypto |
| Backend - Swagger | http://localhost:5166/swagger | Documentación API |
| Backend - Health | http://localhost:5166/api | Status del API |

---

## 📋 Ejemplos de Uso

### 1. Desde el Navegador

**Acceder al módulo de Bonos:**
```
http://localhost:5253/bonos
```

**Formulario:**
- Mercado: `argentina`
- Símbolo: `AL30`
- Click en "Consultar"

**Resultado esperado:**
```
AL30 - 83000 peso_Argentino
Variación: -3.37 %
Apertura: 86500
Máximo: 86900
Mínimo: 83120
Fecha/Hora: 23/10/2025 17:00
Tendencia: sube
```

---

### 2. Usando cURL

#### Obtener cotización de AL30
```bash
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30
```

**Respuesta:**
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
  "puntas": [
    {
      "cantidadCompra": 4026.0,
      "precioCompra": 74500.0,
      "precioVenta": 85000.0,
      "cantidadVenta": 180.0
    }
  ],
  "cantidadOperaciones": 179473
}
```

#### Obtener cotización de AL29
```bash
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL29
```

#### Obtener cotización de GD30
```bash
curl http://localhost:5166/api/bonos/cotizacion/argentina/GD30
```

#### Con formato bonito (requiere jq)
```bash
curl -s http://localhost:5166/api/bonos/cotizacion/argentina/AL30 | jq
```

#### Solo el precio
```bash
curl -s http://localhost:5166/api/bonos/cotizacion/argentina/AL30 | jq '.ultimoPrecio'
```

#### Solo variación
```bash
curl -s http://localhost:5166/api/bonos/cotizacion/argentina/AL30 | jq '.variacion'
```

---

### 3. Usando HTTPie (alternativa moderna a cURL)

```bash
# Instalar httpie
sudo apt install httpie

# Consultar bono
http GET localhost:5166/api/bonos/cotizacion/argentina/AL30
```

---

### 4. Desde PowerShell (Windows)

```powershell
# Consultar bono
Invoke-RestMethod -Uri "http://localhost:5166/api/bonos/cotizacion/argentina/AL30"

# Con formato
Invoke-RestMethod -Uri "http://localhost:5166/api/bonos/cotizacion/argentina/AL30" | ConvertTo-Json
```

---

### 5. Desde JavaScript (Fetch API)

```javascript
// En la consola del navegador
fetch('http://localhost:5166/api/bonos/cotizacion/argentina/AL30')
  .then(res => res.json())
  .then(data => console.log(data));
```

---

### 6. Desde Python

```python
import requests

# Consultar bono
response = requests.get('http://localhost:5166/api/bonos/cotizacion/argentina/AL30')
data = response.json()

print(f"Precio: {data['ultimoPrecio']}")
print(f"Variación: {data['variacion']}%")
print(f"Tendencia: {data['tendencia']}")
```

---

## 📊 Lista de Bonos Disponibles

### Bonos Soberanos Argentinos

| Símbolo | Nombre | Moneda |
|---------|--------|--------|
| AL30 | BONCER 2030 | USD/ARS |
| AL29 | BONCER 2029 | USD/ARS |
| AL35 | BONCER 2035 | USD/ARS |
| AE38 | BONCER 2038 | EUR |
| GD30 | GLOBAL 2030 | USD |
| GD35 | GLOBAL 2035 | USD |
| GD38 | GLOBAL 2038 | USD |
| GD41 | GLOBAL 2041 | USD |
| GD46 | GLOBAL 2046 | USD |

**Mercado:** `argentina`

### Ejemplos de Consulta

```bash
# AL30 - Bono dollar linked 2030
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30

# GD30 - Global 2030
curl http://localhost:5166/api/bonos/cotizacion/argentina/GD30

# AE38 - Bono euro 2038
curl http://localhost:5166/api/bonos/cotizacion/argentina/AE38
```

---

## 🔍 Debugging y Logs

### Ver logs del Backend en tiempo real

**Terminal 3:**
```bash
cd /home/asc/workspace/CryptoPriceSolution
tail -f backend.log
```

Verás algo como:
```
[BonosProvider] Token obtenido exitosamente. Expira en: 23/10/2025 21:19:50
[BonosProvider] Llamando a: https://api.invertironline.com/api/argentina/Titulos/AL30/Cotizacion
[BonosProvider] HTTP 200 - Response: {"ultimoPrecio":83000.0,"variacion":-3.37...
```

### Ver logs del Frontend en tiempo real

**Terminal 4:**
```bash
cd /home/asc/workspace/CryptoPriceSolution
tail -f frontend.log
```

### Verificar que servicios están corriendo

```bash
# Verificar puerto 5166 (backend)
lsof -i :5166

# Verificar puerto 5253 (frontend)
lsof -i :5253

# O con netstat
netstat -tulpn | grep 5166
netstat -tulpn | grep 5253
```

---

## 🧪 Pruebas Manuales

### Test 1: Backend está corriendo
```bash
curl http://localhost:5166/api/bonos/cotizacion/argentina/AL30
```
✅ Debería retornar JSON con la cotización

### Test 2: Frontend está corriendo
```bash
curl http://localhost:5253
```
✅ Debería retornar HTML de Blazor

### Test 3: Frontend puede conectar al Backend
1. Abrir: http://localhost:5253/bonos
2. Abrir Developer Tools (F12)
3. Ir a pestaña "Network"
4. Click en "Consultar"
5. Verificar request a: `http://localhost:5166/api/bonos/cotizacion/...`
6. Ver respuesta HTTP 200

### Test 4: Token se obtiene correctamente
```bash
# Ver logs del backend
tail -f backend.log | grep "Token obtenido"
```
✅ Debería mostrar: `[BonosProvider] Token obtenido exitosamente...`

---

## 🔧 Comandos Útiles

### Compilar proyectos

```bash
# Backend
cd CryptoPriceBackend
dotnet build

# Frontend
cd CryptoPriceFrontendWasm
dotnet build

# Ambos desde la raíz
dotnet build
```

### Limpiar y recompilar

```bash
# Backend
cd CryptoPriceBackend
dotnet clean
dotnet build

# Frontend
cd CryptoPriceFrontendWasm
dotnet clean
dotnet build
```

### Ver versión de .NET

```bash
dotnet --version
```

### Restaurar paquetes NuGet

```bash
dotnet restore
```

### Ver dependencias del proyecto

```bash
# Backend
cd CryptoPriceBackend
dotnet list package

# Frontend
cd CryptoPriceFrontendWasm
dotnet list package
```

---

## 📝 Scripts de Automatización

### test_bonos.sh

Ya creado, ejecutar con:
```bash
./test_bonos.sh
```

### Script personalizado para consultar múltiples bonos

```bash
#!/bin/bash
# Archivo: consultar_bonos.sh

BONOS=("AL30" "AL29" "GD30" "AE38" "AL35")

echo "Consultando bonos argentinos..."
echo ""

for bono in "${BONOS[@]}"; do
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "Bono: $bono"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    
    RESPONSE=$(curl -s "http://localhost:5166/api/bonos/cotizacion/argentina/$bono")
    
    if [ $? -eq 0 ]; then
        PRECIO=$(echo $RESPONSE | jq -r '.ultimoPrecio')
        VARIACION=$(echo $RESPONSE | jq -r '.variacion')
        MONEDA=$(echo $RESPONSE | jq -r '.moneda')
        
        echo "Precio: $PRECIO $MONEDA"
        echo "Variación: $VARIACION%"
    else
        echo "Error consultando $bono"
    fi
    
    echo ""
done
```

**Uso:**
```bash
chmod +x consultar_bonos.sh
./consultar_bonos.sh
```

---

## 🐛 Solución de Problemas Comunes

### Error: "Authorization has been denied for this request"

**Causa:** Credenciales incorrectas o token expirado

**Solución:**
1. Verificar `appsettings.json`:
   ```bash
   cat CryptoPriceBackend/appsettings.json | jq '.InvertirOnline'
   ```
2. Verificar credenciales
3. Reiniciar el backend (el token se renovará automáticamente)

---

### Error: "Connection refused" al consultar backend desde frontend

**Causa:** Backend no está corriendo o puerto incorrecto

**Solución:**
```bash
# Verificar que backend esté corriendo
lsof -i :5166

# Si no está corriendo, iniciarlo
cd CryptoPriceBackend
dotnet run
```

---

### Error: Frontend no carga

**Causa:** Puerto 5253 ocupado o frontend no compilado

**Solución:**
```bash
# Verificar puerto
lsof -i :5253

# Limpiar y recompilar
cd CryptoPriceFrontendWasm
dotnet clean
dotnet build
dotnet run
```

---

### Error: "No se encontró información para el bono"

**Causa:** Símbolo o mercado incorrecto, o API externa caída

**Solución:**
1. Verificar símbolo (debe ser mayúsculas: AL30, no al30)
2. Verificar mercado (debe ser: argentina)
3. Probar con otro bono conocido: AL30, AL29, GD30
4. Ver logs del backend para más detalles

---

## 📚 Recursos Adicionales

### Documentación

- [BONOS_README.md](BONOS_README.md) - Documentación completa
- [BONOS_TECHNICAL_SUMMARY.md](BONOS_TECHNICAL_SUMMARY.md) - Resumen técnico
- [ARCHITECTURE_DIAGRAM.md](ARCHITECTURE_DIAGRAM.md) - Diagrama de arquitectura

### Swagger

Abrir en navegador:
```
http://localhost:5166/swagger
```

Aquí puedes ver y probar todos los endpoints disponibles, incluyendo el de Bonos.

---

## 🎯 Casos de Uso Comunes

### 1. Monitoreo de un bono específico

```bash
# Consultar AL30 cada 30 segundos
watch -n 30 'curl -s http://localhost:5166/api/bonos/cotizacion/argentina/AL30 | jq ".ultimoPrecio"'
```

### 2. Comparar precios de varios bonos

```bash
echo "Comparación de bonos argentinos:"
echo "AL30: $(curl -s http://localhost:5166/api/bonos/cotizacion/argentina/AL30 | jq '.ultimoPrecio')"
echo "AL29: $(curl -s http://localhost:5166/api/bonos/cotizacion/argentina/AL29 | jq '.ultimoPrecio')"
echo "GD30: $(curl -s http://localhost:5166/api/bonos/cotizacion/argentina/GD30 | jq '.ultimoPrecio')"
```

### 3. Guardar cotización histórica

```bash
# Guardar en archivo JSON
curl -s http://localhost:5166/api/bonos/cotizacion/argentina/AL30 > cotizacion_$(date +%Y%m%d_%H%M%S).json
```

### 4. Alertas de precio

```bash
#!/bin/bash
PRECIO_OBJETIVO=85000
PRECIO_ACTUAL=$(curl -s http://localhost:5166/api/bonos/cotizacion/argentina/AL30 | jq '.ultimoPrecio')

if (( $(echo "$PRECIO_ACTUAL > $PRECIO_OBJETIVO" | bc -l) )); then
    echo "¡ALERTA! AL30 superó $PRECIO_OBJETIVO: $PRECIO_ACTUAL"
    # Aquí podrías enviar un email, notificación, etc.
fi
```

---

## 🔐 Seguridad

### Credenciales en producción

**NO usar appsettings.json en producción**

**Usar variables de entorno:**
```bash
export InvertirOnline__Username="tu_usuario"
export InvertirOnline__Password="tu_contraseña"
export InvertirOnline__GrantType="password"
export InvertirOnline__BaseUrl="https://api.invertironline.com"
```

**O Azure Key Vault / AWS Secrets Manager**

---

¡Listo! Ahora tienes todo lo necesario para usar el módulo de Bonos. 🎉
