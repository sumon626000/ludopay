LudoPay - PC on korar por sohoje sob run korar niyom
====================================================

OPTION 1 - DOCKER (recommended)
-------------------------------
Desktop > ludopay folder > start-docker.bat (double-click)

  Needs: Docker Desktop only
  - MongoDB        : localhost:27017
  - Game server    : http://localhost:3000
  - Admin panel    : http://127.0.0.1:8000/admin
  - Login          : admin@gmail.com / NixSumon@Ludo2026

  Stop: stop-docker.bat
  Logs: ludopay-docker\logs.bat
  Guide: SETUP-LOCAL.md


OPTION 2 - EK CLICK (no Docker, local PHP+Node)
-----------------------------------------------
Desktop > ludopay folder > start-all-local.bat (double-click)

  Needs: Node.js + PHP + MongoDB (Docker mongo or local)
  IMPORTANT: Do-ta CMD window BONDHO korben na!


OPTION 2 - PC ON HOLE AUTO START (ekbar setup)
--------------------------------------------
1. install-startup.bat  (double-click, ekbar)
2. PC restart korun
3. Windows login korar por server + browser nije khulbe
   (taskbar-e minimized "LudoPay" CMD window 2-ta)

   Band korte: uninstall-startup.bat


OPTION 3 - Alada alada
----------------------
  start-server-local.bat  = Game only (3000)
  start-admin.bat         = Admin only (8000)
  stop-all-local.bat      = Sob bondho


Unity game
----------
  Project: LudoPayprojectFIle
  Socket : ws://127.0.0.1:3000/socket.io/?EIO=3&transport=websocket

LOCAL SETUP (first time)
------------------------
  1. setup-local.bat       = MongoDB + .env + seed (one time)
  2. start-all-local.bat   = Game + Admin (every time)
  3. stop-all-local.bat    = Stop all

  Full guide: SETUP-LOCAL.md
