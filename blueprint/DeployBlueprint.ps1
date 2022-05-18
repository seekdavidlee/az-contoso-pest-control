# Use this script to deploy Blueprint which will create the necessary resource groups in your environment
# and assigning the Contributor role to the Service principal in those resource groups.

param(
    [string]$BUILD_ENV = "dev",
    [Parameter(Mandatory = $true)][string]$SVC_PRINCIPAL_ID,
    [Parameter(Mandatory = $true)][string]$PLAT_PRINCIPAL_ID,
    [string]$PREFIX = "cpc01",
    [string]$BP_VERSION = "1.2")

$ErrorActionPreference = "Stop"

if (((az extension list | ConvertFrom-Json) | Where-Object { $_.name -eq "blueprint" }).Length -eq 0) {
    az extension add --upgrade -n blueprint
    if ($LastExitCode -ne 0) {
        throw "An error has occured. Unable to add extension."
    }
}

$blueprintName = "cpc$BUILD_ENV"

$subscriptionId = (az account show --query id --output tsv)
if ($LastExitCode -ne 0) {
    throw "An error has occured. Subscription id query failed."
}

$outputs = (az deployment sub create --name "deploy-$blueprintName" --location 'centralus' --template-file blueprint.bicep `
        --subscription $subscriptionId `
        --parameters stackEnvironment=$BUILD_ENV svcPrincipalId=$SVC_PRINCIPAL_ID platPrincipalId=$PLAT_PRINCIPAL_ID `
        blueprintName=$blueprintName prefix=$PREFIX | ConvertFrom-Json)

if ($LastExitCode -ne 0) {
    throw "An error has occured. Deployment failed."
}

$msg = (git log --oneline -n 1)

$outputs.properties.outputs.blueprints.value | ForEach-Object {
    $blueprintName = $_.name

    $blueprintJson = az blueprint publish --blueprint-name $blueprintName --version $BP_VERSION --change-notes $msg --subscription $subscriptionId

    if ($LastExitCode -ne 0) {
        throw "An error has occured. Publish failed."
    }

    $blueprintId = ($blueprintJson | ConvertFrom-Json).id

    $assignmentName = "assign-$blueprintName-$BP_VERSION"

    if ($BUILD_ENV -eq "dev") {
        if ($_.allResourcesDoNotDeleteInDev) {
            $lockMode = "AllResourcesDoNotDelete"
        }
        else {
            $lockMode = "None"
        }
    }
    else {
        $lockMode = "AllResourcesDoNotDelete"
    }

    az blueprint assignment create --subscription $subscriptionId --name $assignmentName `
        --location centralus --identity-type SystemAssigned --blueprint-version $blueprintId `
        --parameters "{}" --locks-mode $lockMode

    if ($LastExitCode -ne 0) {
        throw "An error has occured. Assignment failed."
    }

    az blueprint assignment wait --subscription $subscriptionId --name $assignmentName --created
    if ($LastExitCode -ne 0) {
        throw "An error has occured. Assignment failed (wait)."
    }
}

# This portion of the script handles the role assignments between the managed identities and shared resources.
$ids = az identity list | ConvertFrom-Json
if ($LastExitCode -ne 0) {
    throw "An error has occured. Identity listing failed."
}

$ids = $ids | Where-Object { $_.tags.'stack-name' -eq 'cpc-identity' }

$platformRes = (az resource list --tag stack-name='cpc-shared-key-vault' | ConvertFrom-Json)
if (!$platformRes) {
    throw "Unable to find eligible platform resource!"
}

if ($platformRes.Length -eq 0) {
    throw "Unable to find 'ANY' eligible platform resource!"
}

# Platform specific Azure Key Vault as a Shared resource
$akvid = ($platformRes | Where-Object { $_.type -eq 'Microsoft.KeyVault/vaults' -and $_.tags.'stack-environment' -eq $BUILD_ENV }).id

$ids | ForEach-Object {
    $id = $_
    az role assignment create --assignee $id.principalId --role 'Key Vault Secrets User' --scope $akvid
    if ($LastExitCode -ne 0) {
        throw "An error has occured on role assignment."
    }
}