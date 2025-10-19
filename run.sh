#!/bin/bash
# Script para levantar backend y frontend de CryptoPriceSolution

set -e

# Ruta al backend y frontend
BACKEND_DIR="CryptoPriceBackend"
FRONTEND_DIR="CryptoPriceFrontendWasm"

# Levantar backend en segundo plano
cd "$BACKEND_DIR"
echo "Iniciando backend..."
dotnet watch run &
BACKEND_PID=$!
cd ..

# Levantar frontend en segundo plano
cd "$FRONTEND_DIR"
echo "Iniciando frontend..."
dotnet watch run &
FRONTEND_PID=$!
cd ..

echo "Backend PID: $BACKEND_PID"
echo "Frontend PID: $FRONTEND_PID"
echo "Ambos servicios están corriendo con hot reload."
echo "Para detenerlos, usa: kill $BACKEND_PID $FRONTEND_PID"

# Esperar a que ambos procesos terminen
wait $BACKEND_PID $FRONTEND_PID
