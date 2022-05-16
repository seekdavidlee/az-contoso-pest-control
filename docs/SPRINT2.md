# Introduction
The second sprint is focused on establishing governance practices (such as managing shared resources, Networking, RBAC) with Azure Blueprint in Azure Subscription.

To follow along, checkout the following.
```
git checkout tags/sprint2 -b sprint2
```

# Architecture
We will create an Azure Blueprint to define the resource groups, role assignments to the resource group and policy assignments such as what Azure regions we allow resources to be created. We will also create shared resources which will be used across all Cloud projects going forward. The Azure Blueprint will be executed by the Platform team responsible for maintaining the Azure Subscription. We will also need to ensure networking is in place.

1. There will be 3 resource groups creatd initially based on which teams is responsible for the resource groups.
    1. Shared resources resource group: This is where we will create resources that will be shared across cloud projects, and not just for this microservices app dev team. This is managed by the Platform team.
        1. RBAC enabled Azure Key Vault: All secrets and certs used by the App itself will be stored here with appropriate role assignments. 
    2. Networking resource group: This is where we will create 2 VNETs - one primary and one secondary for DR purposes. We plan to leverage Central US as primary and East US 2 which is [paired](https://docs.microsoft.com/en-us/azure/availability-zones/cross-region-replication-azure) with it. There will be 3 subnets created, one as default, one for Application Gateway and one for Azure Kubernetes Service which are planned for later in [Sprint 4](docs/SPRINT4.md). This is managed by the Networking team.
    3. API resource group: This is for all microservice related resources such as Azure Kubernetes Service and Azure SQL. This is managed by the App Dev team.
2. From a role assignment perspective, we have 2 types of users Azure Blueprint will require. The first one is the Platform team who needs to be assigned admin level assignments to any resources. The second type is our Service Principals we have created with mostly contributor access. We will be passing in their principal Ids as parameters in this context.
    1. We understand we need to enhance Azure Blueprint for App Dev team and Networking team later to take care of their role assignments, but for this initial round, we will focus on just the first two type of users described.  
3. We plan to create 2 environments, one for DEV and one for PROD. Hence, when we execute Azure Blueprint, we will need to pass in the environment parameter. 
4. Versioning is a concept built into Azure Blueprint but we control what version format to follow. For us, we have decided on using a MAJOR and MINOR version. When we are developing our Azure Blueprint, we will increment with MINOR version and when we are ready to release, we will increment with a MAJOR. This can be passed in as one of the parameter.
5. Azure Blueprint will allow us to document any work we are doing in the Azure Subscription as code. This means any time we need to make a change to the Subscription, it needs to follow the process of an engineer branching off in git, making the changes, and doing a Pull Request to merge the changes into main. This means everything we do is aduited and reviewed by another engineer. Once merged into Prod, the engineer is able to execute the Azure Blueprint with the appropriate parameters. As a quick note, all changes executed with Azure Blueprint is also documented in the Auzre Subsription and we can also check there as well.
6. From a networking perspective, we are going to leverage Network Security Groups to prevent access to our VNETs except from our office location. As such, there will be a need to configure a source IP parameter.

## Implementation
We have decided on using Bicep as the language for creating our Azure Blueprint (VS using ARM Template which is a JSON syntax). This will make it easier for our engineers as it is much more readable. We have also decide to go with using Azure CLI for the scripting portion as it appears to be more robust with the number of Azure Services it can be used with and it has a better tooling around output format (VS Azure PowerShell). That said, we are ready with the implementations steps!

1. Ensure you have the latest version of bicep.
```
az bicep upgrade
```
2. The first step is to create a Service Principal which is assigned into each resource group. Take note of the tenant Id, appId and password.
```
az ad sp create-for-rbac -n "Contoso Pest Control GitHub"
``` 
3. We need to get the Object Id for the Service principal we have created. This is used as input to our Blueprint deployment later as the SVC_PRINCIPAL_ID parameter.
```
az ad sp show --id <appId from the previous command> --query "objectId" | ConvertFrom-Json
```
4. We need to create a group for all Platform team members in Azure Active Directory. If it is just yourself, you can use the command below for the PLAT_PRINCIPAL_ID parameter used later.
```
az ad signed-in-user show --query 'objectId' | ConvertFrom-Json
```
5. We should cd into the blueprint directory and execute our blueprint.bicep with the following command.
```
DeployBlueprint.ps1 -SVC_PRINCIPAL_ID <Object Id for Contoso Pest Control GitHub Service Principal> -PLAT_PRINCIPAL_ID <Object Id for your group>
```
6. When this is completed, login to the Azure Portal to review the results of the Azure Blueprint deployment. If there is an issue, we can always remove the assignment, update the defination and rerun. The version should still be incremented.
7. We will also need to make sure the appropriate Azure Resources are created with the expected role assignments.
8. Lastly, we run the following command to deploy our networking resources in this Sprint. We expect to ensure it is part of the CD process later in [sprint 4](docs/SPRINT4.md). Notice we have selected to deploy to our dev environment which is why we are using the dev resource group and stackEnvironment as dev. Also be sure to set your source IP which is to represent your office location.
```
az deployment group create -n deploy-1 -g cpc-networking-dev --template-file deployment/networking.bicep --parameters stackEnvironment=dev sourceIp=$allowedIP
```

## Troubleshooting
1. When running the blueprint, you may encounter an error saying the resource name is already in use. If so, you need to rollback all blueprint assignments and blueprints, remove the resource group and provide a Prefix parameter with a different name. For example, use ```cpc123 ``` as the default is ``` cpc ```.
2. If you encounter an error related to resource is in a updated state when running the networking bicep deployment, you may need to wait several minutes before re-running the same command again with a different deployment name.

## Next Step
[Sprint 3: Setting up DevOps CI (Continuous Integration) with Unit Testing and pushing container builds into Azure Container Registry (ACR) with GitHub Actions](../docs/SPRINT3.md)