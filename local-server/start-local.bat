@echo off
cd /d %~dp0
if not exist node_modules (
  echo Installing local dependency...
  call npm install
)
echo Starting OSC Fader local server...
node server.js
pause
