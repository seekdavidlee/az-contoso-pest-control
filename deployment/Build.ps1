param($APP_NAME, $APP_VERSION, $BUILD_ENV, $BUILD_PATH)

$ErrorActionPreference = "Stop"

$platformRes = (az resource list --tag stack-name=cpc-shared-container-registry | ConvertFrom-Json)
if (!$platformRes) {
    throw "Unable to find eligible container registry!"
}
if ($platformRes.Length -eq 0) {
    throw "Unable to find 'ANY' eligible platform container registry!"
}

$acr = ($platformRes | Where-Object { $_.tags.'stack-environment' -eq $BUILD_ENV })
if (!$acr) {
    throw "Unable to find eligible container registry!"
}
$AcrName = $acr.Name

az acr login --name $AcrName
if ($LastExitCode -ne 0) {
    throw "An error has occured. Unable to login to acr."
}

$shouldBuild = $true
$tags = az acr repository show-tags --name $AcrName --repository $APP_NAME | ConvertFrom-Json
if ($tags) {
    if ($tags.Contains($APP_VERSION)) {
        $shouldBuild = $false
    }
}

if ($shouldBuild -eq $true) {
    # Build your app with ACR build command
    $imageName = "$APP_NAME`:$APP_VERSION"
    Write-Host "Image name: $imageName"
    az acr build --image $imageName -r $AcrName --file ./$BUILD_PATH/Dockerfile .

    if ($LastExitCode -ne 0) {
        throw "An error has occured. Unable to build image."
    }
}