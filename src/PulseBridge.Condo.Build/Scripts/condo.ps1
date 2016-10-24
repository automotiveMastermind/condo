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

function Download-File([string] $url, [string] $path, [int] $retries = 5) {
    try {
        Invoke-WebRequest $url -OutFile $path | Out-Null
        break
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

# find the script path
$RootPath = $PSScriptRoot

# set well-known paths
$WorkingPath = Convert-Path (Get-Location)
$ArtifactsRoot = Join-Path $WorkingPath "artifacts"
$BuildRoot = Join-Path $WorkingPath ".build"

$MSBuildPath = Join-Path $BuildRoot "msbuild-cli"
$MSBuildProj = Join-Path $MSBuildPath "project.json"
$MSBuildPublish = Join-Path $MSBuildPath "bin\publish"
$MSBuildLog = Join-Path $BuildRoot "condo.msbuild.log"
$MSBuildRsp = Join-Path $BuildRoot "condo.msbuild.rsp"

$CondoPath = Join-Path $BuildRoot "condo"
$CondoPublish = $MSBuildPublish
$CondoLog = Join-Path $BuildRoot "condo.log"

if (!(Test-Path $ArtifactsRoot)) {
    mkdir $ArtifactsRoot | Out-Null
}

if (Test-Path $CondoLog) {
    del -Force $CondoLog
}

if (Test-Path $MSBuildLog) {
    del -Force $MSBuildLog
}

if (Test-Path $MSBuildRsp) {
    del -Force $MSBuildRsp
}

$DotNetPath = Join-Path $env:LOCALAPPDATA "Microsoft\dotnet"

function Execute-Cmd([string] $cmd) {
    # get the command name
    $cmdName = [System.IO.Path]::GetFileName($cmd)

    # execute the command
    & $cmd @args 2>&1 >> $CondoLog

    # capture the exit code
    $exitCode = $LASTEXITCODE

    # determine if the command was successful
    if($exitCode -ne 0) {
        # throw an exception message
        throw "'$cmdName $args' failed with exit code: $exitCode. Check '$CONDO_LOG' for additional information..."
    }
}

function Install-DotNet() {
    $dotnetUrl = $env:DOTNET_INSTALL_URL
    $dotnetChannel = $env:DOTNET_CHANNEL
    $dotnetVersion = $env:DOTNET_VERSION

    if (!$dotnetUrl) {
        $dotnetUrl = "https://github.com/dotnet/cli/raw/rel/1.0.0/scripts/obtain/dotnet-install.ps1"
    }

    if (!$dotnetChannel) {
        $dotnetChannel = "rel-1.0.0"
    }

    if (!$dotnetVersion) {
        $dotnetVersion = "1.0.0-preview2-003121"
    }

    if ($env:SKIP_DOTNET_INSTALL) {
        Write-Info "Skipping installation of dotnet-cli by request (SKIP_DOTNET_INSTALL is set)..."
        $env:PATH = "$env:PATH;$DotNetPath"
    }
    else {
        $dotnetTemp = Join-Path ([System.IO.Path]::GetTempPath()) $([System.IO.Path]::GetRandomFileName())
        $dotnetInstall = Join-Path $dotnetTemp "condo.zip"

        try {
            mkdir $dotnetTemp | Out-Null
            Download-File -url $Uri -Path $dotnetInstall
            Execute-Cmd "$dotnetInstall" -Channel $dotnetChannel -Version $dotnetVersion -Architecture $env:PROCESSOR_ARCHITECTURE
        }
        finally {
            del -Force $dotnetTemp
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
        Write-Info "condo was already build: use -Reset to get the latest version."
        return
    }

    try {
        mkdir $MSBuildPath | Out-Null

        $runtime = ((& dotnet --info) | Select-String -pattern "RID:[\s]+(.*)$").Matches[1].Value
    }
}
