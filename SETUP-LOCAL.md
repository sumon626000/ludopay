# Monster Game — Local Setup with Docker

Everything runs in Docker: **MongoDB + Game Server + Laravel Admin**.

## Requirements

- **Docker Desktop** (Windows): https://www.docker.com/products/docker-desktop
- Enable WSL2 when Docker asks

No need to install Node.js or PHP on Windows (runs inside containers).

---

## Quick start

### 1. First time
Double-click:
```
setup-local.bat
```
or
```
start-docker.bat
```

### 2. Stop
```
stop-docker.bat
```

### 3. View logs
```
ludopay-docker\logs.bat
```

---

## URLs

| Service | URL |
|---------|-----|
| Admin panel | http://127.0.0.1:8000/admin |
| Game server | http://localhost:3000 |
| MongoDB | localhost:27017 |

**Login:** `admin@gmail.com` / `NixSumon@Ludo2026`

**Unity socket:**
```
ws://127.0.0.1:3000/socket.io/?EIO=3&transport=websocket
```

---

## Docker containers

| Container | Port | Role |
|-----------|------|------|
| `ludo-mongo` | 27017 | MongoDB |
| `ludo-server` | 3000 | Node.js game server |
| `ludo-admin` | 8000 | Laravel admin |

---

## Manual commands

```bat
cd ludopay-docker
docker compose up -d --build
docker compose ps
docker compose logs -f admin
docker compose down
```

Reset MongoDB (delete all data):
```bat
cd ludopay-docker
docker compose down -v
docker compose up -d --build
```

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| Docker not running | Open Docker Desktop, wait until green |
| Port 3000/8000 busy | `stop-docker.bat` then start again |
| Admin 500 | `docker compose logs admin` — wait 30s for seed |
| Guest login fail | Check `docker compose logs server` — need "Connected to the DB" |
| Build slow first time | Normal — downloads Node + PHP images |

---

## Without Docker (alternative)

Use `start-all-local.bat` — requires Node.js + PHP installed locally.
See old flow in `README-START.txt`.

Production server setup: `WEBUZO-SETUP.md`
