# LudoPay — AdMob Reward Button Setup

## Admin panel (monstergame-admin)

1. Run backend (`backend`, port 4000) and admin panel (`admin-panel`, port 3000).
2. Open **Settings → AdMob** (`/settings/admob`).
3. Configure:
   - **AdMob App ID**
   - **Rewarded Ad Unit (Android)** (or default rewarded unit)
   - **Reward coins per ad**
   - **Cooldown (hours)**
   - **Ads Enabled** / **Test Mode**

Unity reads: `GET {AdminApiBaseUrl}/mobile/config` → `data.admob`.

Default API base: `http://localhost:4000/api`  
Override in app: `PlayerPrefs` key `AdminApiBaseUrl`.

## Unity scene

1. Menu: **LudoPay → Create Reward Ad Button Prefab** → `Assets/Prefabs/UI/RewardButton.prefab`
2. Add empty GameObject `RewardAdManager` to MenuScene (or first load creates one from button).
3. Drag **RewardButton** prefab onto **Home** panel.
4. On `LoginManager` / menu load, config is fetched automatically via `MenuManager` + `RewardAdManager`.

## Google Mobile Ads SDK (real ads)

1. Install [Google Mobile Ads Unity plugin](https://github.com/googleads/googleads-mobile-unity) compatible with Unity 6.
2. **Player Settings → Android → Scripting Define Symbols** add: `ENABLE_ADMOB`
3. Do **not** add `ENABLE_ADMOB` to **GUEST_BUILD** Android builds (guest strip keeps Firebase safe).
4. **Assets → External Dependency Manager → Android Resolver → Resolve**

## Android manifest

In `Assets/Plugins/Android/AndroidManifest.xml` inside `<application>`:

```xml
<meta-data
    android:name="com.google.android.gms.ads.APPLICATION_ID"
    android:value="ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy"/>
```

Replace with your AdMob App ID from the admin panel.

## GUEST_BUILD

- `GUEST_BUILD` = no AdMob native code compiled; reward button still shows cooldown from config but will not load ads on device.
- Use full build + `ENABLE_ADMOB` for production ads.

## Socket server (optional)

LudoPay also uses `REQ_GAME_SETTINGS` on the socket server. You may mirror `admob` fields there later; HTTP config is the supported path for admin panel updates.
