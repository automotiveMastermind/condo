#requires -version 4

# find the script path
$RootPath = $PSScriptRoot

# setup well-known paths
$CondoScriptPath = Join-Path $RootPath "condo-local.ps1"
$TemplatePath = Join-Path $RootPath "template"
$CondoTemplatePath = Join-Path $TemplatePath "condo.ps1"

# determine if condo-local.ps1 already exists and delete it if it does
if (Test-Path $CondoScriptPath) {
    del -Force $CondoScriptPath
}

# copy the template to the local path
cp $CondoTemplatePath $CondoScriptPath | Out-Null

# run condo using local build
try {
    # change to the root path
    pushd $RootPath

    # execute the local script
    & $CondoScriptPath -Reset -Local @args
}
finally {
    # change back to the current path
    popd

    # remove the local condo script
    del -Force $CondoScriptPath
}
