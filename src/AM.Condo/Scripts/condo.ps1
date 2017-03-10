#requires -version 4

[CmdletBinding(PositionalBinding=$false, HelpUri = 'http://open.automotivemastermind.com/condo')]

Param (
    [Parameter()]
    [Alias('nc')]
    [switch]
    $NoColor,

    [Parameter()]
    [Alias('v')]
    [ValidateSet('Quiet', 'Minimal', 'Normal', 'Detailed', 'Diagnostic')]
    [string]
    $Verbosity = 'Normal',

    [Parameter()]
    [string]
    $Username,

    [Parameter()]
    [SecureString]
    $Password,

    [Parameter(ValueFromRemainingArguments)]
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
        Invoke-WebRequest $url -OutFile $path > $null
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

$ArtifactsRoot = "$WorkingPath\artifacts"
$CondoRoot = "$HOME\.am\condo"
$SrcRoot = "$CondoRoot\.src"
$BuildRoot = "$CondoRoot\.build"

$MSBuildLog = "$BuildRoot\condo.msbuild.log"
$MSBuildRsp = "$BuildRoot\condo.msbuild.rsp"

$CondoPath = "$SrcRoot\src\AM.Condo"
$CondoPublish = "$CondoRoot\cli"
$CondoLog = "$BuildRoot\condo.log"
$CondoTargets = "$CondoPath\Targets"
$CondoProj = "$WorkingPath\condo.build"

if (!(Test-Path $ArtifactsRoot)) {
    New-Item $ArtifactsRoot -ItemType Directory > $null
}

if (Test-Path $BuildRoot) {
    Remove-Item $BuildRoot -Recurse -Force > $null
}

New-Item $BuildRoot -ItemType Directory -> $null

if (Test-Path $CondoLog) {
    Remove-Item $CondoLog -Force > $null
}

if (Test-Path $MSBuildLog) {
    Remove-Item $MSBuildLog -Force > $null
}

if (Test-Path $MSBuildRsp) {
    Remove-Item $MSBuildRsp -Force > $null
}

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
        return
    }

    $dotnetTemp = Join-Path ([System.IO.Path]::GetTempPath()) $([System.IO.Path]::GetRandomFileName())
    $dotnetInstall = Join-Path $dotnetTemp "dotnet-install.ps1"

    try {
        New-Item $dotnetTemp -ItemType Directory > $null
        Get-File -url $dotnetUrl -Path $dotnetInstall
        Invoke-Cmd "$dotnetInstall" -Channel $dotnetChannel -Version $dotnetVersion
    }
    finally {
        Remove-Item -Recurse -Force $dotnetTemp > $null
    }

    if (!($env:Path.Split(';') -icontains $DotNetPath)) {
        $env:PATH = "$DotNetPath;$env:PATH"
    }

    Write-Success "dotnet-cli was installed..."
}

function Install-Condo() {
    if (Test-Path $CondoPublish) {
        Write-Info "condo was already built: use -Reset to get the latest version."
        return
    }

    # create the condo publish path
    New-Item $CondoPublish -ItemType Directory -ErrorAction SilentlyContinue > $null

    # get the runtime
    $runtime = ((& dotnet --info) | Select-String -pattern "RID:[\s]+(.*)$").Matches.Groups[1].Value

    # restore msbuild
    Write-Info "condo: restoring condo packages..."
    Invoke-Cmd dotnet restore $SrcRoot --runtime $runtime --verbosity minimal
    Write-Success "condo: restore complete"

    # publish condo
    Write-Info "condo: publishing condo tasks..."
    Invoke-Cmd dotnet publish $CondoPath --runtime $runtime --output $CondoPublish --verbosity minimal
    Write-Success "condo: publish complete"
}

try
{
    Install-DotNet
    Install-Condo

    $MSBuildRspData = @"
/nologo
"$CondoProj"
/p:CondoTargetsPath="$CondoTargets\\"
/p:CondoTasksPath="$CondoPublish\\"
/p:PackageFeedUsername="$Username"
/p:PackageFeedPassword="$Password"
/fl
/flp:LogFile="$MSBuildLog";Encoding=UTF-8;Verbosity=$Verbosity
/clp:$MSBuildDisableColor;Verbosity=$Verbosity
"@

    $MSBuildRspData | Out-File -Encoding ASCII -FilePath $MSBuildRsp
    $MSBuildArgs | foreach { $_ | Out-File -Append -Encoding ASCII -FilePath $MSBuildRsp }

    Write-Info "Starting build..."
    Write-Info "msbuild '$CondoProj' $MSBuildArgs"

    & "dotnet" "msbuild" `@"$MSBuildRsp"
}
finally {
    Copy-Item $MSBuildRsp, $CondoLog, $MSBuildLog -Destination $ArtifactsRoot -ErrorAction SilentlyContinue > $null
}
