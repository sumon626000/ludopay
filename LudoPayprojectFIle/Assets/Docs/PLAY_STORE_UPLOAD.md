# Google Play Store Upload — Monster Game

## One-click fix (Unity)

Menu: **LudoPay → Apply Play Store Compliance (Fix All)**

Then: **File → Build Settings → Android → Build App Bundle (Release)**

## Virtual coins only (NOT real money)

This game does **not** use real money:
- No cash withdraw (Paytm / bank)
- No real-money deposit
- Only **virtual coins** for playing Ludo

Wallet / withdraw UI is hidden in all builds.
- Removes **test AdMob App ID**
- Sets **HTTPS-only** (no cleartext traffic)
- Uses **GUEST_BUILD** (no Firebase/Facebook native crash)
- App name: **Monster Game**

## Play Console (you must fill manually)

1. **Privacy policy URL:** https://admin.monstergame.app/privacy-policy
2. **Data safety:** phone, email, profile photo, game activity
3. **App category:** Game
4. **Target audience:** 18+ if any social features with UGC

## Optional: enable AdMob later

1. Put real App ID in `Assets/Resources/PlayStore/admob_app_id.txt`
2. Add `ENABLE_ADMOB` to Android Scripting Define Symbols
3. Run **Apply Play Store Compliance** again
4. Rebuild AAB

## Server requirements for Play Store build

- Admin API: **https://admin.monstergame.app/api**
- Game socket: **wss://** (secure WebSocket), not `ws://`
