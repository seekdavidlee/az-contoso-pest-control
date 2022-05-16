# Introduction
The third sprint is focused on setting up DevOps CI (Continuous Integration) with Unit Testing and pushing container builds into Azure Container Registry (ACR) with GitHub Actions.

To follow along, checkout the following.
```
git checkout tags/sprint3 -b sprint3
```

# Architecture
We will now include a GitHub Action workflow that will run the unit tests as well as push to the Azure Container Reigstry if the unit test are passing. This will be excluded from main branch and any md files as they are just docs.

## Implementation

1. We will make use of GitHub secrets to store Service Principal details in a specified environment. Review the secrets section below and add it to both dev and prod environments.
2. Next, we should create a dev branch and push to remote to kick off the GitHub Action.
3. We should review the GitHub Action and review Azure Container Registry (ACR) to ensure the image is created as a repository in ACR.

## Secrets
| Name | Value |
| --- | --- |
| AZURE_CREDENTIALS | <pre>{<br/>&nbsp;&nbsp;&nbsp;&nbsp;"clientId": "",<br/>&nbsp;&nbsp;&nbsp;&nbsp;"clientSecret": "", <br/>&nbsp;&nbsp;&nbsp;&nbsp;"subscriptionId": "",<br/>&nbsp;&nbsp;&nbsp;&nbsp;"tenantId": "" <br/>}</pre> |