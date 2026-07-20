# LudoPay – Docker One-Click Setup

এক কমান্ডে Node server + MongoDB চালু হবে। Unity project আলাদা থাকবে — শুধু APK এর server URL ঠিক রাখলেই হবে।

## ১) Prerequisite (একবারই)

[Docker Desktop](https://www.docker.com/products/docker-desktop) install করুন (Windows/Mac/Linux)।

## ২) Folder Structure

এই zip extract করার পর folder টা এমন দেখাবে:

```
ludopay-docker/
├── docker-compose.yml
├── Dockerfile.server
├── start.bat   stop.bat   logs.bat   reset-db.bat
├── start.sh
├── .env.example
├── backup/                     ← (optional) MongoDB dump এখানে রাখুন
└── LudoPayServer/              ← আপনার Node server ফোল্ডার এখানে কপি করুন
    ├── package.json
    ├── bin/www
    └── ...
```

> **গুরুত্বপূর্ণ:** আপনার `LudoPayServer` ফোল্ডারটা (যেটাতে `package.json` ও `bin/www` আছে) এই ডিরেক্টরির ভেতরে কপি করুন।

## ৩) Database Backup (optional)

পুরাতন MongoDB থেকে dump নিতে:

```
mongodump --uri="mongodb://localhost:27017/ludo" --out=./backup
```

`backup/` ফোল্ডারটা এই project এর ভেতরে রাখলে, প্রথমবার চালুর সময় auto restore হবে।

## ৪) Run (এক ক্লিক)

**Windows:** `start.bat` ডাবল ক্লিক
**Mac/Linux:** `bash start.sh`

প্রথমবার ৩-৫ মিনিট লাগবে (image download + npm install)। এরপর প্রতিবার ৫ সেকেন্ডে চালু হবে।

চালু হওয়ার পর:

| Service | URL |
|---|---|
| Node game server | `http://localhost:3000` |
| Admin API | `http://localhost:16000` |
| MongoDB | `mongodb://localhost:27017/ludo` |

## ৫) Lovable Admin Site কীভাবে connect করবে?

আপনার Lovable admin app এ login screen এ:
- **Server URL:** `http://localhost:16000` (একই PC থেকে)
  অথবা `http://আপনার-PC-IP:16000` (LAN থেকে)
- **Username/Password:** `.env` ফাইলে যা সেট করেছেন (default: `admin / admin123`)

## ৬) Unity APK কোথায় Server URL দেবে?

Unity তে যে script এ API call হয় (যেমন `ApiManager.cs` / `ServerConfig.cs`):

```csharp
public static string ServerURL = "http://আপনার-PC-IP:3000";
```

PC এর LAN IP বের করতে: `ipconfig` (Windows) → IPv4 Address।

## ৭) Server code change করলে?

`LudoPayServer/` ফোল্ডারের কোড volume-mount করা — file save করলেই container এর ভেতরে আপডেট হবে। Restart দরকার হলে:

```
docker compose restart server
```

## ৮) Useful Commands

| কাজ | Command |
|---|---|
| Start | `start.bat` বা `docker compose up -d` |
| Stop | `stop.bat` বা `docker compose down` |
| Logs | `logs.bat` বা `docker compose logs -f server` |
| DB reset + re-restore | `reset-db.bat` |
| Mongo shell | `docker exec -it ludo-mongo mongosh ludo` |

## ৯) Troubleshooting

- **Port already in use:** `docker-compose.yml` এ port mapping পরিবর্তন করুন (যেমন `"3001:3000"`)।
- **Connection refused from phone:** PC firewall এ port 3000 allow করুন এবং phone-PC একই WiFi তে আছে কিনা দেখুন।
- **Admin login fail:** `.env` এর username/password চেক করুন, তারপর `docker compose restart server`।
