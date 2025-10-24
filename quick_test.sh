#!/bin/bash

echo "Iniciando backend..."
cd /home/asc/workspace/CryptoPriceSolution/CryptoPriceBackend
dotnet run > /tmp/backend.log 2>&1 &
BACKEND_PID=$!

echo "Backend PID: $BACKEND_PID"
echo "Esperando 10 segundos para que el backend inicie..."
sleep 10

echo ""
echo "Probando endpoint de bonos..."
echo "URL: http://localhost:5166/api/bonos/cotizacion/argentina/AL30"
echo ""

curl -v http://localhost:5166/api/bonos/cotizacion/argentina/AL30

echo ""
echo ""
echo "Deteniendo backend..."
kill $BACKEND_PID

echo ""
echo "Logs del backend:"
cat /tmp/backend.log
