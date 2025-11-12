#!/bin/bash

# Script para iniciar Backend y Frontend de CryptoPriceSolution con análisis de bonos
# Autor: GitHub Copilot
# Fecha: 2025-10-25

echo "========================================"
echo "  CryptoPriceSolution - Bonos Charts"
echo "========================================"
echo ""

# Colores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Directorio base
BASE_DIR="/home/asc/workspace/CryptoPriceSolution"

# Verificar que estamos en el directorio correcto
if [ ! -d "$BASE_DIR" ]; then
    echo -e "${RED}Error: No se encuentra el directorio $BASE_DIR${NC}"
    exit 1
fi

cd "$BASE_DIR"

# Función para verificar si un puerto está en uso
check_port() {
    local port=$1
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1 ; then
        return 0  # Puerto en uso
    else
        return 1  # Puerto libre
    fi
}

# Función para detener procesos en un puerto
kill_port() {
    local port=$1
    echo -e "${YELLOW}Deteniendo procesos en puerto $port...${NC}"
    lsof -ti:$port | xargs kill -9 2>/dev/null || true
    sleep 2
}

echo "1. Verificando puertos..."
echo ""

# Verificar y liberar puertos si están ocupados
if check_port 5166; then
    echo -e "${YELLOW}Puerto 5166 (Backend) está ocupado${NC}"
    kill_port 5166
fi

if check_port 5253; then
    echo -e "${YELLOW}Puerto 5253 (Frontend) está ocupado${NC}"
    kill_port 5253
fi

echo -e "${GREEN}✓ Puertos disponibles${NC}"
echo ""

echo "2. Compilando Backend..."
cd "$BASE_DIR/CryptoPriceBackend"
dotnet build --configuration Release > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Backend compilado exitosamente${NC}"
else
    echo -e "${RED}✗ Error compilando Backend${NC}"
    exit 1
fi
echo ""

echo "3. Compilando Frontend..."
cd "$BASE_DIR/CryptoPriceFrontendWasm"
dotnet build --configuration Release > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Frontend compilado exitosamente${NC}"
else
    echo -e "${RED}✗ Error compilando Frontend${NC}"
    exit 1
fi
echo ""

echo "4. Iniciando servicios..."
echo ""

# Crear directorio para logs
mkdir -p "$BASE_DIR/logs"

# Iniciar Backend en background
echo -e "${GREEN}▶ Iniciando Backend en http://localhost:5166${NC}"
cd "$BASE_DIR/CryptoPriceBackend"
nohup dotnet run --urls "http://localhost:5166" > "$BASE_DIR/logs/backend.log" 2>&1 &
BACKEND_PID=$!
echo "   PID: $BACKEND_PID"
echo ""

# Esperar a que el backend inicie
echo -e "${YELLOW}Esperando a que el Backend inicie...${NC}"
for i in {1..30}; do
    if curl -s http://localhost:5166 > /dev/null 2>&1; then
        echo -e "${GREEN}✓ Backend iniciado correctamente${NC}"
        break
    fi
    sleep 1
    if [ $i -eq 30 ]; then
        echo -e "${RED}✗ Timeout esperando al Backend${NC}"
        kill $BACKEND_PID 2>/dev/null
        exit 1
    fi
done
echo ""

# Iniciar Frontend en background
echo -e "${GREEN}▶ Iniciando Frontend en http://localhost:5253${NC}"
cd "$BASE_DIR/CryptoPriceFrontendWasm"
nohup dotnet run --urls "http://localhost:5253" > "$BASE_DIR/logs/frontend.log" 2>&1 &
FRONTEND_PID=$!
echo "   PID: $FRONTEND_PID"
echo ""

# Esperar a que el frontend inicie
echo -e "${YELLOW}Esperando a que el Frontend inicie...${NC}"
for i in {1..30}; do
    if curl -s http://localhost:5253 > /dev/null 2>&1; then
        echo -e "${GREEN}✓ Frontend iniciado correctamente${NC}"
        break
    fi
    sleep 1
    if [ $i -eq 30 ]; then
        echo -e "${RED}✗ Timeout esperando al Frontend${NC}"
        kill $BACKEND_PID $FRONTEND_PID 2>/dev/null
        exit 1
    fi
done
echo ""

echo "========================================"
echo -e "${GREEN}  ✓ Servicios Iniciados Exitosamente${NC}"
echo "========================================"
echo ""
echo "📊 URLs disponibles:"
echo ""
echo "   Frontend:        http://localhost:5253"
echo "   Backend API:     http://localhost:5166"
echo "   Swagger:         http://localhost:5166/swagger"
echo ""
echo "🎯 Páginas de Bonos:"
echo ""
echo "   Cotización:      http://localhost:5253/bonos"
echo "   Histórico:       http://localhost:5253/bonos-charts"
echo ""
echo "📝 Logs:"
echo ""
echo "   Backend:         $BASE_DIR/logs/backend.log"
echo "   Frontend:        $BASE_DIR/logs/frontend.log"
echo ""
echo "🔧 PIDs de procesos:"
echo ""
echo "   Backend PID:     $BACKEND_PID"
echo "   Frontend PID:    $FRONTEND_PID"
echo ""
echo "⚠️  Para detener los servicios ejecuta:"
echo ""
echo "   ./close.sh"
echo "   O manualmente: kill $BACKEND_PID $FRONTEND_PID"
echo ""
echo "========================================"
echo ""

# Guardar PIDs para el script de cierre
echo "$BACKEND_PID" > "$BASE_DIR/.backend.pid"
echo "$FRONTEND_PID" > "$BASE_DIR/.frontend.pid"

# Opción: Abrir navegador automáticamente
read -p "¿Deseas abrir el navegador automáticamente? (s/n): " -n 1 -r
echo ""
if [[ $REPLY =~ ^[SsYy]$ ]]; then
    echo -e "${GREEN}Abriendo navegador...${NC}"
    
    # Detectar el navegador disponible
    if command -v xdg-open > /dev/null; then
        xdg-open http://localhost:5253/bonos-charts > /dev/null 2>&1 &
    elif command -v gnome-open > /dev/null; then
        gnome-open http://localhost:5253/bonos-charts > /dev/null 2>&1 &
    elif command -v firefox > /dev/null; then
        firefox http://localhost:5253/bonos-charts > /dev/null 2>&1 &
    elif command -v google-chrome > /dev/null; then
        google-chrome http://localhost:5253/bonos-charts > /dev/null 2>&1 &
    else
        echo -e "${YELLOW}No se pudo detectar un navegador. Abre manualmente:${NC}"
        echo "http://localhost:5253/bonos-charts"
    fi
fi

echo ""
echo -e "${GREEN}¡Todo listo! Disfruta analizando bonos 🚀${NC}"
echo ""

# Mantener el script corriendo para mostrar logs en tiempo real
echo "Presiona Ctrl+C para detener el monitoreo de logs (los servicios seguirán corriendo)"
echo ""
tail -f "$BASE_DIR/logs/backend.log" "$BASE_DIR/logs/frontend.log"
