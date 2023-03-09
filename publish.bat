@echo off
echo building...
dotnet build

echo starting...
cd bin/Debug/net6.0-windows
REFramework.exe
rem Steamworks.exe --base_path "./SteamAppSample/" --steam_appid 367500 --steam_api64 "base_path" --steam_api "base_path"
rem Steamworks.exe --path "./emu/" --appid 367500 --steamApi64Dll "./emu/" --steamApiDll "./emu/"
