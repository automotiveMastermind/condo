@echo off
cd %~dp0

ECHO.

SETLOCAL
    SET DNXPATH=%USERPROFILE%\.dnx
    SET DNVMPATH=%DNXPATH%\dnvm
    SET DNVMCMD=%DNVMPATH%\dnvm.cmd
    SET DNVMPS1=%DNVMPATH%\dnvm.ps1

    SET DNVMCMDURI="https://raw.githubusercontent.com/aspnet/Home/dev/dnvm.cmd"
    SET DNVMPS1URI="https://raw.githubusercontent.com/aspnet/Home/dev/dnvm.ps1"

    IF NOT EXIST "%DNVMPATH%" (
        mkdir "%DNVMPATH%"
    )

    IF NOT EXIST "%DNVMPS1%" (
	   @powershell -NoProfile -ExecutionPolicy Unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%DNVMPS1URI%' -OutFile '%DNVMPS1%' -UseBasicParsing"
    )

    IF NOT EXIST "%DNVMCMD%" (
        @powershell -NoProfile -ExecutionPolicy Unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%DNVMCMDURI%' -OutFile '%DNVMCMD%' -UseBasicParsing"

        CALL "%DNVMCMD%" update-self
    )

    CALL "%DNVMCMD%" upgrade -r coreclr -nonative

    SET NUGETPATH=%AGENT_BUILDDIRECTORY%\NuGet

    IF "%AGENT_BUILDDIRECTORY%" == "" (
        SET NUGETPATH=%LOCALAPPDATA%\NuGet
    )

    SET NUGETCMD=%NUGETPATH%\nuget.exe
    SET NUGETURI="https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

    IF NOT EXIST "%NUGETPATH%" (
        mkdir "%NUGETPATH%"
    )

    IF NOT EXIST "%NUGETCMD%" (
        @powershell -NoProfile -ExecutionPolicy Unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%NUGETURI%' -OutFile '%NUGETCMD%'"
    )

    SET NUGETROOT=.nuget
    SET NUGET=%NUGETROOT%\nuget.exe

    IF NOT EXIST "%NUGETROOT%" (
        mkdir "%NUGETROOT%"
    )

    IF NOT EXIST "%NUGET%" (
        copy "%NUGETCMD%" "%NUGET%"
    )

    SET SAKEPKG=packages\Sake
    SET SAKE=%SAKEPKG%\tools\Sake.exe
    SET INCLUDES=src\build\sake
    SET MAKE=make.shade

    IF EXIST "%SAKEPKG%" (
<<<<<<< HEAD
        rd "%SAKEPKG%" /s /q
=======
        rd "%SAKEPKG% /s /q
>>>>>>> origin/release/1.0.0
    )

    "%NUGET%" install Sake -pre -o packages -ExcludeVersion

<<<<<<< HEAD
    ECHO.

=======
>>>>>>> origin/release/1.0.0
    "%SAKE%" -I "%INCLUDES%" -f "%MAKE%" %*
ENDLOCAL
