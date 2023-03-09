@echo off

if exist "%PROGRAMFILES(X86)%" (goto x64) else (goto x86)

:x64
echo x64
start "" "%PROGRAMFILES%\Git\bin\sh.exe" --login
goto batend

:x86
echo x86
start "" "%SYSTEMDRIVE%\Program Files (x86)\Git\bin\sh.exe" --login
goto batend

:batend