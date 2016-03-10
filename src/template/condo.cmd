@ECHO OFF
PUSHD %~dp0

ECHO.

SETLOCAL
SETLOCAL ENABLEDELAYEDEXPANSION

    IF NOT DEFINED VisualStudioVersion (
        IF DEFINED VS140COMNTOOLS (
            CALL "%VS140COMNTOOLS%\VsDevCmd.bat"
            ECHO USING VISUAL STUDIO 2015 TOOLS
            GOTO :EnvironmentReady
        )

        IF DEFINED VS120COMNTOOLS (
            CALL "%VS120COMNTOOLS%\VsDevCmd.bat"
            ECHO USING VISUAL STUDIO 2013 TOOLS
            GOTO :EnvironmentReady
        )

        IF DEFINED VS110COMNTOOLS (
            CALL "%VS110COMNTOOLS%\VsDevCmd.bat"
            ECHO USING VISUAL STUDIO 2012 TOOLS
            GOTO :EnvironmentReady
        )
    )

    :EnvironmentReady
    SET DNXPATH=%USERPROFILE%\.dnx
    SET DNVMPATH=%DNXPATH%\dnvm
    SET DNVMCMD=%DNVMPATH%\dnvm.cmd
    SET DNVMPS1=%DNVMPATH%\dnvm.ps1

    SET DNVMCMDURI="https://raw.githubusercontent.com/aspnet/Home/dev/dnvm.cmd"
    SET DNVMPS1URI="https://raw.githubusercontent.com/aspnet/Home/dev/dnvm.ps1"

    IF NOT EXIST "%DNVMPATH%" (
        MKDIR "%DNVMPATH%"
    )

    IF NOT EXIST "%DNVMPS1%" (
	   @powershell -NoProfile -ExecutionPolicy Unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%DNVMPS1URI%' -OutFile '%DNVMPS1%' -UseBasicParsing"
    )

    IF NOT EXIST "%DNVMCMD%" (
        @powershell -NoProfile -ExecutionPolicy Unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%DNVMCMDURI%' -OutFile '%DNVMCMD%' -UseBasicParsing"

        CALL "%DNVMCMD%" update-self
    )

    SET NUGETPATH=%AGENT_BUILDDIRECTORY%\NuGet

    IF [%AGENT_BUILDDIRECTORY%] == [] (
        SET NUGETPATH=%LOCALAPPDATA%\NuGet
    )

    SET NUGETCMD=%NUGETPATH%\nuget.exe
    SET NUGETURI="https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

    IF NOT EXIST "%NUGETPATH%" (
        MKDIR "%NUGETPATH%"
    )

    IF NOT EXIST "%NUGETCMD%" (
        @powershell -NoProfile -ExecutionPolicy Unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%NUGETURI%' -OutFile '%NUGETCMD%'"
    )

    SET NUGETROOT=.nuget
    SET NUGET=%NUGETROOT%\nuget.exe

    IF NOT EXIST "%NUGETROOT%" (
        MKDIR "%NUGETROOT%"
    )

    IF NOT EXIST "%NUGET%" (
        COPY "%NUGETCMD%" "%NUGET%"
    )

    CALL "%DNVMCMD%" install latest -r coreclr -a x86 -nonative -alias default
    CALL "%DNVMCMD%" install latest -r clr -a x86 -nonative -alias default

    SET FEEDSRC=%CONDO_NUGET_SRC%
    SET SAKEPKG=packages\Sake
    SET SAKE=%SAKEPKG%\tools\Sake.exe
    SET CONDOPKG=packages\PulseBridge.Condo
    SET CONDO=%CONDOPKG%\PulseBridge.Condo.nuspec
    SET INCLUDES=%CONDOPKG%\build
    SET MAKE=condo.shade

    IF [%FEEDSRC%] == [] (
        SET FEEDSRC=https://api.nuget.org/v3/index.json
    )

    IF NOT EXIST "%SAKE%" (
        "%NUGET%" install Sake -pre -o packages -ExcludeVersion -NonInteractive
    )

    IF NOT EXIST "%CONDOPKG%" (
        "%NUGET%" install PulseBridge.Condo -pre -o packages -ExcludeVersion -NonInteractive -Source "%FEEDSRC%"
    )

    ECHO.

    "%SAKE%" -I "%INCLUDES%" -f "%MAKE%" %*

    IF ["%1"] == ["update-self"] (
        RMDIR /S /Q "%SAKEPKG%" 1>NUL 2>&1
        RMDIR /S /Q "%CONDOPKG%" 1>NUL 2>&1
    )
ENDLOCAL

POPD
ECHO.