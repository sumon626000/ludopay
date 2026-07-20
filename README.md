# Monster Game (LudoPay)

Virtual-coin Ludo game: Unity client, Laravel admin, Node.js Socket.IO server.

## Local (Windows + Docker)

```bat
start-docker.bat
```

- Admin: http://127.0.0.1:8000/admin
- Game server: http://localhost:3000
- Login: `admin@gmail.com` / `NixSumon@Ludo2026`

See `SETUP-LOCAL.md` and `README-START.txt`.

## Webuzo production deploy

**Full guide:** [`WEBUZO-FULL-GUIDE.md`](WEBUZO-FULL-GUIDE.md)

1. Push this repo to GitHub
2. On Webuzo SSH: `git clone YOUR_REPO ludopay-repo`
3. Run: `bash webuzo/setup-all.sh`

Database included: `database-backup/webplustechludo.zip`

## Project layout

| Folder | Description |
|--------|-------------|
| `LudoPayprojectFIle/` | Unity game project |
| `laravel-admin/` | Laravel admin panel |
| `ludopay-docker/` | Docker (MongoDB, server, admin) |
| `database-backup/` | MongoDB dump for Webuzo import |
| `webuzo/` | Webuzo deploy scripts |
