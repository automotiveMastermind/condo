#requires -version 4

[CmdletBinding(PositionalBinding=$false, HelpUri = 'http://open.pulsebridge.com/condo')]

Param (
    [Parameter(Mandatory=$false)]
    [Alias("nc")]
    [switch]
    $NoColor,

    [Parameter(Mandatory=$false)]
    [Alias("v")]
    [ValidateSet("Quiet", "Minimal", "Normal", "Detailed", "Diagnostic")]
    [string]
    $Verbosity = "Normal",

    [Parameter(Mandatory=$false, ValueFromRemainingArguments=$true)]
    [string[]]
    $MSBuildArgs
)

function Write-Message([string] $message, [System.ConsoleColor] $color) {
    if ($NoColor) {
        Write-Host $message
        return
    }

    Write-Host -ForegroundColor $color $message
}

function Write-Success([string] $message) {
    Write-Message -Color Green -Message $message
}

function Write-Failure([string] $message) {
    Write-Message -Color Red -Message $message
}

function Write-Info([string] $message) {
    Write-Message -Color Yellow -Message $message
}

function Get-File([string] $url, [string] $path, [int] $retries = 5) {
    try {
        Invoke-WebRequest $url -OutFile $path | Out-Null
    }
    catch [System.Exception]
    {
        Write-Failure "Unable to retrieve file: '$url'"

        if ($retries -eq 0) {
            $exception = $_.Exception
            throw $exception
        }

        Write-Failure "Retrying in 10 seconds... attempts left: $retries"
        Start-Sleep -Seconds 10
        $retries--
    }
}

# set well-known paths
$WorkingPath = Convert-Path (Get-Location)
$ArtifactsRoot = Join-Path $WorkingPath "artifacts"
$BuildRoot = Join-Path $WorkingPath ".build"

$MSBuildPath = Join-Path $BuildRoot "msbuild-cli"
$MSBuildProj = Join-Path $MSBuildPath "project.json"
$MSBuildPublish = Join-Path (Join-Path $MSBuildPath "bin") "publish"
$MSBuildLog = Join-Path $BuildRoot "condo.msbuild.log"
$MSBuildRsp = Join-Path $BuildRoot "condo.msbuild.rsp"

$CondoPath = Join-Path (Join-Path $BuildRoot "condo") "PulseBridge.Condo"
$CondoPublish = $MSBuildPublish
$CondoLog = Join-Path $BuildRoot "condo.log"
$ScriptsPath = Join-Path $CondoPath "Scripts"

if (Test-Path $ArtifactsRoot) {
	del -Force -Recurse $ArtifactsRoot | Out-Null
}

if (Test-Path $CondoLog) {
    del -Force $CondoLog | Out-Null
}

if (Test-Path $MSBuildLog) {
    del -Force $MSBuildLog | Out-Null
}

if (Test-Path $MSBuildRsp) {
    del -Force $MSBuildRsp | Out-Null
}

mkdir $ArtifactsRoot | Out-Null

$DotNetPath = Join-Path $env:LOCALAPPDATA "Microsoft\dotnet"
$MSBuildDisableColor = ""

if ($NoColor) {
    $MSBuildDisableColor = "DisableConsoleColor"
}

function Invoke-Cmd([string] $cmd) {
    # get the command name
    $cmdName = [System.IO.Path]::GetFileName($cmd)

    # execute the command
    & $cmd @args 2>&1 >> $CondoLog

    # capture the exit code
    $exitCode = $LASTEXITCODE

    # determine if the command was successful
    if($exitCode -ne 0) {
        # throw an exception message
        $message = "'$cmdName $args' failed with exit code: $exitCode. Check '$CondoLog' for additional information..."
        $exception = New-Object System.FormatException $message
        Write-Failure $message
        throw $exception
    }
}

function Install-DotNet() {
    $dotnetUrl = $env:DOTNET_INSTALL_URL
    $dotnetChannel = $env:DOTNET_CHANNEL
    $dotnetVersion = $env:DOTNET_VERSION

    if (!$dotnetUrl) {
        $dotnetUrl = "https://github.com/dotnet/cli/raw/rel/1.0.1/scripts/obtain/dotnet-install.ps1"
    }

    if (!$dotnetChannel) {
        $dotnetChannel = "rel-1.0.1"
    }

    if (!$dotnetVersion) {
        $dotnetVersion = "1.0.1"
    }

    if ($env:SKIP_DOTNET_INSTALL) {
        Write-Info "Skipping installation of dotnet-cli by request (SKIP_DOTNET_INSTALL is set)..."
        $env:PATH = "$env:PATH;$DotNetPath"
    }
    else {
        $dotnetTemp = Join-Path ([System.IO.Path]::GetTempPath()) $([System.IO.Path]::GetRandomFileName())
        $dotnetInstall = Join-Path $dotnetTemp "dotnet-install.ps1"

        try {
            mkdir $dotnetTemp | Out-Null
            Get-File -url $dotnetUrl -Path $dotnetInstall
            Invoke-Cmd "$dotnetInstall" -Channel $dotnetChannel -Version $dotnetVersion
        }
        finally {
            del -Recurse -Force $dotnetTemp
        }

        Write-Success "dotnet-cli was installed..."

        if (!($env:Path.Split(';') -icontains $DotNetPath)) {
            $env:PATH = "$DotNetPath;$env:PATH"
        }
    }

    $sharedPath = (Join-Path (Split-Path ((get-command dotnet.exe).Path) -Parent) "shared");
    (Get-ChildItem $sharedPath -Recurse *dotnet.exe) | %{ $_.FullName } | Remove-Item;
}

function Install-MSBuild() {
    if (Test-Path $MSBuildPath) {
        Write-Info "condo was already built: use -Reset to get the latest version."
        return
    }

    try {
        mkdir $MSBuildPublish -ErrorAction SilentlyContinue | Out-Null
        mkdir $CondoPublish -ErrorAction SilentlyContinue | Out-Null

        $runtime = ((& dotnet --info) | Select-String -pattern "RID:[\s]+(.*)$").Matches.Groups[1].Value

        $contents = [System.IO.File]::ReadAllText((Convert-Path "$ScriptsPath\msbuild.json"))
        [System.IO.File]::WriteAllText($MSBuildProj, $contents.Replace("RUNTIME", $runtime))

        # restore msbuild
        Write-Info "condo: restoring condo packages..."
        Invoke-Cmd dotnet restore $BuildRoot --verbosity minimal
        Write-Success "condo: restore complete"

        # publish condo
        Write-Info "condo: publishing condo tasks..."
        Invoke-Cmd dotnet publish $CondoPath --output $CondoPublish --runtime $runtime
        Write-Success "condo: publish complete"

        # publish msbuild
        Write-Info "msbuild: publishing msbuild system..."
        Invoke-Cmd dotnet publish $MSBuildPath --output $MSBuildPublish --runtime $runtime
        Write-Success "msbuild: publish complete"
    }
    catch {
        $ex = $error[0]

        if(Test-Path $MSBuildPath) {
            del -rec -for $MSBuildPath
        }

        throw $ex
    }
}

try
{
    Install-DotNet
    Install-MSBuild

    $CondoTargets = Join-Path $CondoPath "Targets"
    $CondoProj = Join-Path $WorkingPath "condo.build"

    if (!(Test-Path $CondoProj)) {
        $CondoProj = Join-Path $CondoTargets "condo.build"
    }

    $MSBuildRspData = @"
/nologo
"$CondoProj"
/p:CondoTargetsPath="$CondoTargets\\"
/p:CondoTasksPath="$CondoPublish\\"
/fl
/flp:LogFile="$MSBuildLog";Encoding=UTF-8;Verbosity=$Verbosity
/clp:$MSBuildDisableColor;Verbosity=$Verbosity
"@

    $MSBuildRspData | Out-File -Encoding ASCII -FilePath $MSBuildRsp
    $MSBuildArgs | foreach { $_ | Out-File -Append -Encoding ASCII -FilePath $MSBuildRsp }

    Write-Info "Starting build..."
    Write-Info "msbuild '$CondoProj' $MSBuildArgs"

    & "$MSBuildPublish\corerun.exe" "$MSBuildPublish\MSBuild.dll" `@"$MSBuildRsp"
}
finally {
    cp -ErrorAction SilentlyContinue $MSBuildRsp,$CondoLog,$MSBuildLog $ArtifactsRoot
}
