#requires -version 4
function Write-Message([string] $message, [System.ConsoleColor] $color) {
    if ($NoColor)
    {
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

$ContainerName = "automotivemastermind/condo"

try
{
    Write-Info "Checking if docker exists"
    #check if docker exists
    if(Get-Command 'docker' -errorAction SilentlyContinue)
    {
        Write-Info "Docker was found!"
        #check global.json for sdk version
        $config = Get-Content -Raw -Path ${pwd}/global.json | ConvertFrom-Json
        if($config.sdk.version -match "[0-9]")
        {
            #docker exists lets check what container system we are using
            if(docker version | Where-Object {$_ | Select-String "linux"})
            {
                Write-Info "Running with linux containers"
                docker run -it -v ${pwd}:/app ($ContainerName + ":unix-core" + $matches[0]) bash -c ./condo.sh /t:publish
            }
            else
            {
                Write-Info "Running with windows containers"
                docker run -it -v ${pwd}:/app ($ContainerName + ":win-core" + $matches[0]) powershell -c ./condo.ps1 /t:publish
            }
        }
    }
    else
    {
        #docker does not exist run condo without container
        Invoke-Cmd ./condo.ps1
    }
}
finally
{

}
