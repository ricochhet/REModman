@echo off
mkdir "./REMod/bin/Debug/net7.0-windows10.0.22621.0/Tools"
robocopy "./REMod.RisePakPatch/bin/Debug/net472/" "./REMod/bin/Publish/Tools/" RisePakPatch.exe
pause