#Requires -version 4


<#
.SYNOPSIS
    Builds the project using the condo build system.
.DESCRIPTION
    Uses condo to build the project(s) contained within the current repository. If condo is not already present, it will
    be downloaded and restored using the provided URI or branch. If not URL or branch is provided, the latest release
    version will be downloaded.
.PARAMETER Reset
    Deletes the pre-existing locally restored copy of condo and its dependencies before redownloading and restoring.
.PARAMETER Local
    Uses the current repository to restore condo and its dependencies. This is useful for locally testing customizations
    to condo from its own repository.
.PARAMETER Uri
    The URI from which to download and restore condo.
.PARAMETER Branch
    The branch from which to download and restore condo from its default repository.
.PARAMETER Source
    The file system path from which to restore condo and its dependencies from source. This is useful for locally testing
    customizations to condo from a different repository.
.PARAMETER Verbosity
    The verbosity used for messaging to the standard output from both condo and the underlying MSBuild system.

	Acceptable values are: Quiet, Minimal, Normal, Detailed, and Diagnostic
.PARAMETER NoColor
    Indicates that any messaging to the standard output should not be emitted using colors. This is useful for parsing
    output by third party tools.
.PARAMETER Bootstrap
	Indicates that condo should bootstrap a developer environment to configure access to secured package feeds.
.PARAMETER Username
	The username used to bootstrap access to secured package feeds.
.PARAMETER Password
	The password used to bootstrap access to secured package feeds. For Visual Studio Team Services (VSTS) feeds, this
	should be an access token with at least the Packaging (read) scope.
.PARAMETER MSBuildArgs
	Contains any additional parameters that are not bound to one of the parameters above. These values will be passed
	to the underlying MSBuild runtime. These values are automatically bound from all remaining arguments. Specifying
	the parameter as a collection is not necessary. See examples for more information.
.EXAMPLE
    condo -Uri https://github.com/pulsebridge/condo/releases/2.0.0.zip

	# use the specified uri to install condo (if it is not already installed)
.EXAMPLE
    condo -Branch develop

	# use the develop branch to install condo
.EXAMPLE
    condo -Reset -Verbosity Detailed

	# reset condo to latest release build and enable verbose logging
.EXAMPLE
	condo /t:Publish /p:Configuration=Release

	# pass a target and property to the msbuild runtime
.EXAMPLE
	condo -Bootstrap -Username bill.gates@contoso.com -Password B528BF58-8C1F-48AC-9D9D-737E5DFD2B77

	# bootstrap secured feeds using the specified username and password
.INPUTS
	None. Condo does not accept any inputs through a pipe.
.OUTPUTS
	None. Condo does not emit any outputs through a pipe.
.NOTES
    The underlying build system in use is Microsoft Build for .NET Core. Any parameters beyond those supported by this
    cmdlet will be passed to the msbuild process for consideration.
.LINK
	http://open.pulsebridge.com/condo
#>


[CmdletBinding(DefaultParameterSetName='ByBranch', PositionalBinding=$false)]
Param (
    [Parameter(Mandatory=$false)]
    [Alias("r")]
    [switch]
    $Reset,

    [Parameter(Mandatory=$false)]
    [Alias("l")]
    [switch]
    $Local,

    [Parameter(Mandatory=$false)]
    [Alias("nc")]
    [switch]
    $NoColor,

	[Parameter(Mandatory=$true, ParameterSetName="Bootstrap")]
	[switch]
	$Bootstrap,

	[Parameter(Mandatory=$true, ParameterSetName="Bootstrap")]
	[string]
	$Username,

	[Parameter(Mandatory=$true, ParameterSetName="Bootstrap")]
	[string]
	$Password,

    [Parameter(Mandatory=$false)]
    [Alias("v")]
    [ValidateSet("Quiet", "Minimal", "Normal", "Detailed", "Diagnostic")]
    [string]
    $Verbosity = "Normal",

    [Parameter(Mandatory=$true, ParameterSetName="ByUri")]
    [Alias("u")]
    [string]
    $Uri,

    [Parameter(Mandatory=$false, ParameterSetName="ByBranch")]
    [Alias("b")]
    [string]
    $Branch = "develop",

    [Parameter(Mandatory=$true, ParameterSetName="BySource")]
    [Alias("s")]
    [string]
    $Source,

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

# find the script path
$RootPath = $PSScriptRoot

$BuildRoot = Join-Path $RootPath ".build"
$CondoRoot = Join-Path $BuildRoot "condo"
$CondoScript = Join-Path $CondoRoot "PulseBridge.Condo\Scripts\condo.ps1"

if ($PSCmdlet.ParameterSetName -eq "ByBranch") {
    $Uri = "https://github.com/pulsebridge/condo/archive/$Branch.zip"
}

if ($Reset -and (Test-Path $BuildRoot)) {
    Write-Info "Resetting condo build system..."
    del -Recurse -Force $BuildRoot | Out-Null
}

if ($Local) {
    $Source = Join-Path $RootPath "src"
}

if (!(Test-Path $CondoRoot)) {
    Write-Info "Creating path for condo at $CondoRoot"
    mkdir $CondoRoot | Out-Null

    if ($Source) {
        Write-Info "Using condo build system from $Source"
        cp -Recurse "$Source\*" $CondoRoot
    } else {
        Write-Info "Using condo build system from $Uri"

        $CondoTemp = Join-Path ([System.IO.Path]::GetTempPath()) $([System.IO.Path]::GetRandomFileName())
        $CondoZip = Join-Path $CondoTemp "condo.zip"

        mkdir $CondoTemp | Out-Null

        Get-File -url $Uri -Path $CondoZip

        $CondoExtract = Join-Path $CondoTemp "extract"

        Add-Type -AssemblyName System.IO.Compression.FileSystem
        [System.IO.Compression.ZipFile]::ExtractToDirectory($CondoZip, $CondoExtract)

        pushd "$CondoExtract\*\src"
        cp -Recurse * $CondoRoot
        popd

        if (Test-Path $CondoTemp) {
            del -Recurse -Force $CondoTemp
        }
    }
}

try {
    # change to the root path
    pushd $RootPath

	# add the bootstrap arguments
	if ($Bootstrap) {
		$MSBuildArgs = @(
			$MSBuildArgs,
			"/t:Bootstrap",
			"/p:PackageFeedUsername=$Username",
			"/p:PackageFeedPassword=$Password"
		)
	}

	# execute the underlying script
	& "$CondoScript" -Verbosity $Verbosity -NoColor:$NoColor @MSBuildArgs
}
finally {
    popd
}