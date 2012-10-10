@echo OFF
For /f "tokens=2,3,4 delims=. " %%G in ('echo %_version%') Do (set _major=%%G& set _minor=%%H& set _build=%%I) 

if "%_major%"=="1" goto sub1
if "%_major%"=="2" goto sub2
:sub1
AT > NUL
IF %ERRORLEVEL% NEQ 0 (
   echo Access is denied. Please run as Administrator
   PAUSE
   EXIT /B 1
)
powershell.exe  Set-ExecutionPolicy Unrestricted 2>1 >null

IF %ERRORLEVEL% NEQ 0 (
   echo At first install Windows Powershell 
   pause
   exit 1
)
pushd "%~dp0" 
powershell.exe -STA .\login.ps1
@echo ON
goto:eof

:sub2
powershell.exe  Set-ExecutionPolicy Unrestricted 2>1 >null

IF %ERRORLEVEL% NEQ 0 (
   echo At first install Windows Powershell 
   pause
   exit 1
)
pushd "%~dp0" 
powershell.exe -STA .\login.ps1
@echo ON
goto:eof

