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

$DockerLinux = "condo"
$DockerWindows = $DockerLinux+":windows"

try
{
    Write-Info "Checking if docker exists"
    #check if docker exists
    if(Get-Command 'docker' -errorAction SilentlyContinue)
    {
        Write-Info "Docker was found!"
        #docker exists lets check what container system we are using
        if(docker version | Where-Object {$_ | Select-String "linux"})
        {
            Write-Info "Running with linux containers"
            docker run -it -v ${pwd}:/src $DockerLinux bash -c ./condo.sh /t:publish
        }
        else
        {
            Write-Info "Running with windows containers"
            docker run -it -v ${pwd}:/src $DockerWindows powershell -c ./condo.ps1 /t:publish
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
