#!/usr/bin/env bash
set -e
[ -f .env ] || cp .env.example .env
docker compose up -d --build
echo ""
echo "Node Server : http://localhost:3000"
echo "Admin API   : http://localhost:16000"
echo "MongoDB     : localhost:27017"
