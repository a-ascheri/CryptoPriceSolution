#!/bin/bash

# Script para probar el módulo de Bonos

echo "=========================================="
echo "  Probando Módulo de Bonos"
echo "=========================================="
echo ""

# Colores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Verificar que estamos en el directorio correcto
if [ ! -f "CryptoPriceSolution.sln" ]; then
    echo -e "${RED}Error: Este script debe ejecutarse desde el directorio CryptoPriceSolution${NC}"
    exit 1
fi

# Función para iniciar el backend
start_backend() {
    echo -e "${YELLOW}Iniciando Backend en puerto 5166...${NC}"
    cd CryptoPriceBackend
    dotnet run --urls "http://localhost:5166" > ../backend.log 2>&1 &
    BACKEND_PID=$!
    echo $BACKEND_PID > ../backend.pid
    cd ..
    echo -e "${GREEN}Backend iniciado (PID: $BACKEND_PID)${NC}"
    echo "Logs: backend.log"
}

# Función para iniciar el frontend
start_frontend() {
    echo -e "${YELLOW}Iniciando Frontend en puerto 5253...${NC}"
    cd CryptoPriceFrontendWasm
    dotnet run --urls "http://localhost:5253" > ../frontend.log 2>&1 &
    FRONTEND_PID=$!
    echo $FRONTEND_PID > ../frontend.pid
    cd ..
    echo -e "${GREEN}Frontend iniciado (PID: $FRONTEND_PID)${NC}"
    echo "Logs: frontend.log"
}

# Función para detener los servicios
stop_services() {
    echo ""
    echo -e "${YELLOW}Deteniendo servicios...${NC}"
    
    if [ -f backend.pid ]; then
        BACKEND_PID=$(cat backend.pid)
        kill $BACKEND_PID 2>/dev/null
        rm backend.pid
        echo -e "${GREEN}Backend detenido${NC}"
    fi
    
    if [ -f frontend.pid ]; then
        FRONTEND_PID=$(cat frontend.pid)
        kill $FRONTEND_PID 2>/dev/null
        rm frontend.pid
        echo -e "${GREEN}Frontend detenido${NC}"
    fi
}

# Trap para detener servicios al salir
trap stop_services EXIT

# Compilar proyectos
echo -e "${YELLOW}Compilando proyectos...${NC}"
cd CryptoPriceBackend && dotnet build -v q && cd ..
cd CryptoPriceFrontendWasm && dotnet build -v q && cd ..
echo -e "${GREEN}Compilación exitosa${NC}"
echo ""

# Iniciar servicios
start_backend
sleep 3
start_frontend
sleep 5

echo ""
echo "=========================================="
echo -e "${GREEN}Servicios iniciados correctamente${NC}"
echo "=========================================="
echo ""
echo "Backend:  http://localhost:5166"
echo "Frontend: http://localhost:5253"
echo "Bonos:    http://localhost:5253/bonos"
echo ""
echo "=========================================="
echo "  Probando endpoint del backend"
echo "=========================================="
echo ""

# Esperar a que el backend esté listo
echo -e "${YELLOW}Esperando a que el backend esté listo...${NC}"
sleep 8

# Probar endpoint de bonos
echo -e "${YELLOW}Consultando cotización de AL30...${NC}"
echo ""
echo "Request: GET http://localhost:5166/api/bonos/cotizacion/argentina/AL30"
echo ""

RESPONSE=$(curl -s -w "\n%{http_code}" http://localhost:5166/api/bonos/cotizacion/argentina/AL30)
HTTP_CODE=$(echo "$RESPONSE" | tail -n1)
BODY=$(echo "$RESPONSE" | head -n-1)

if [ "$HTTP_CODE" = "200" ]; then
    echo -e "${GREEN}✓ Respuesta exitosa (HTTP $HTTP_CODE)${NC}"
    echo ""
    echo "Respuesta JSON:"
    echo "$BODY" | python3 -m json.tool 2>/dev/null || echo "$BODY"
else
    echo -e "${RED}✗ Error en la respuesta (HTTP $HTTP_CODE)${NC}"
    echo ""
    echo "Respuesta:"
    echo "$BODY"
    echo ""
    echo -e "${YELLOW}Revisando logs del backend...${NC}"
    tail -n 20 backend.log
fi

echo ""
echo "=========================================="
echo "  Instrucciones"
echo "=========================================="
echo ""
echo "1. Abre tu navegador en: http://localhost:5253/bonos"
echo "2. Ingresa:"
echo "   - Mercado: argentina"
echo "   - Símbolo: AL30 (o AL29, GD30, AE38)"
echo "3. Haz clic en 'Consultar'"
echo ""
echo "Para ver logs en tiempo real:"
echo "  Backend:  tail -f backend.log"
echo "  Frontend: tail -f frontend.log"
echo ""
echo "Presiona Ctrl+C para detener los servicios"
echo ""

# Mantener el script corriendo
wait
