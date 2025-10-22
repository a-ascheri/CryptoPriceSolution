#!/bin/bash
# Script para cerrar procesos en los puertos 5166 y 5253

for port in 5166 5253; do
  pid=$(lsof -ti :$port)
  if [ -n "$pid" ]; then
    echo "Cerrando proceso en el puerto $port (PID: $pid)"
    kill $pid
  else
    echo "No hay proceso escuchando en el puerto $port"
  fi
done
