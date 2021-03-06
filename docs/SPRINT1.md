# Introduction
The first sprint is about designing and implementing the Microservice Architecture for the first microservice and setting up local developer's environment.

To follow along, checkout the following.
```
git checkout tags/sprint1 -b sprint1
```

# Architecture
After discussion internally, this is our proposed architecture which speaks to some of the best practices for hosting microservices on the cloud.

1. We will ensure the microservice is stateless in nature which means avoiding authentication cookies and other stateful implementations. For example, in concrete terms, the use of OAuth Bearer token will ensure our microservice remains stateless as each HTTP request would carry that token from a security perspective.
2. We will host a Swagger endpooint for the purpose of exposing documentation around its API. This will allow any team members who is interested in consuming the API to understand the specifications.
3. We will host a health check endpoint and this will allow host services to ensure the microservice is healthy or drop it and create a new instance. This will also ensure host services with capabilities to report issues to trigger any alerts.
4. We will use RESTful conventions which means POST to create, PUT to update, GET to get resources with specific routes and DELETE to remove a resource.
5. We will attempt to use a [sidecar pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/sidecar) to access dependencies so that we can abstract ourselves away from concrete implementation details. The only exception is the database access because we expect our ORM (Object-Relational-Mapping) library to take care of the dependency abstraction. This point speaks to the Pub/Sub service layer which will leverage this pattern.
6. We will ensure that we enable appropriate logging which will ensure Developers to be able to debug and troubleshooot quickly and effectively. 

![Architecture](/src/ContosoPestControl.Issue/Architecture/Contoso-Pest-Control-Microservice.png)

## Implementation
The IT Team has decided to leverage Azure as its Cloud provider and this means we will be heavily invested in key Microsoft techologies. As contractors for the Architecture and Solution, we are also expected to do the initial development work but the IT Team has a few developers who will need to maintain it. This means we need to ensure a smooth Development experience as well. 

1. We will leverage [Docker containers](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/container-docker-introduction/docker-defined) in order to provide the best out-of-box development experience for the IT Team's developers. 
2. We will be using .NET 6 for building the microservice as there is [Long-Term-Support](https://dotnet.microsoft.com/en-us/platform/support/policy) for it.
    * We will use [dotnet Secret Manager tool](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#how-the-secret-manager-tool-works) for managing any secrets such as the database password. As a best practice, we will NEVER check in any secrets so this will be perfect.     
3. We will be using MS SQL as the backend database. As such, we have documented the following command so the IT Team Devs can have a SQL Server instance running on their local dev environment with Docker.
```
docker pull mcr.microsoft.com/mssql/server:2019-latest
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$Password" -p 1433:1433 --name sqldev -h sqldev -d mcr.microsoft.com/mssql/server:2019-latest
```
4. We will leverage [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) as our ORM to access the database. This library also supports built-in retry so we can account for [transient errors](https://docs.microsoft.com/en-us/azure/azure-sql/database/troubleshoot-common-connectivity-issues#transient-errors-transient-faults).
4. We will use [DAPAC](https://docs.microsoft.com/en-us/sql/relational-databases/data-tier-applications/data-tier-applications?view=sql-server-ver15) for migrating database schema and this becomes simple as it can be checked in with code into the GIT repo. From a DevOps perspective, this is a supported migration strategy so we can minimize efforts on how to update database schema moving forward.
    * We will be using the [file secret store](https://docs.dapr.io/operations/components/setup-secret-store/) to access cnnection string to Azure Service Bus. This means having to create a local secrets file and exclude it using gitignore.
5. We will be using Microsoft Authentication Library (MSAL) and Microsoft identity platform for securing access to the Microservice. Again, the objective is to prevent leaking secrets into our source code.
6. For the sidecar pattern, we will be leveraging [DPAR (Distributed Application Runtime)](https://docs.dapr.io/getting-started/install-dapr-cli/). We plan to leverage Azure Service Bus for Pub/Sub to minimize the amount of effort to stand up Pub/Sub components. With DPAR, there should only be [simple YAML configuration](https://docs.dapr.io/reference/components-reference/supported-pubsub/setup-azure-servicebus/) we need to follow.
7. We will use the [built-in health check](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0) interface and libary which will minimize any custom implementation work.
8. We will use the [built-in swagger](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-6.0) which will minimize any custom implementation work.
9. We will leverage the ILogger interface which allows us to track log messages for the purpose of debugging and troubleshoot any exceptions and stack strace.  
10. Testing ensures our code remains reliable with any new changes. For testing:
    * We will use MS Test for writing unit tests to ensure our business logic does not break.
    * We will use POSTMAN for local end-to-end tests to ensure dependencies continue to work as dependencies themselves may have breaking changes that we may not be aware of. POSTMAN will allow us to write tests as well as have a nice output showing test results as part of our DevOps CI process.
11. We will use GitHub as our GIT repository and will follow best practices such as branching where main represents production code and other branches are either feature or bug fixes. We will use Pull Requests for merging code into main. There are plans to use GitHub actions later for building up our CI/CD workflows.

### Local Dev Setup
We are now ready to showcase what we have done and hand over the work the IT Team devs. Here are the steps we have documented.

1. git clone this repo locally and create a branch
2. Docker with sql should be running locally. We should log in with Management Studio and Import our DAPAC into your local Docker SQL instance.
3. Next, we will log into AAD to create 2 App registrations, one for API and one for API Client as we plan to use client credential flow during development to get a bearer token. We also need to create a scope ``` api://contoso-pest-control-api ``` and also a role for application.
4. For working locally, we need to create a local appsettings.json file with the following configs.
```
{
	"AzureAd": {
		"Instance": "https://login.microsoftonline.com/",
		"Domain": "<Domain>.onmicrosoft.com",
		"TenantId": "<Tenant Id>",
		"ClientId": "<Client Id>",
		"Audience": "api://contoso-pest-control-api"
	},
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.AspNetCore": "Warning"
		}
	},
	"DbUsername": "sa",
	"DbServer": "localhost",
	"DbName": "<Db name>",
	"PubsubName": "contoso-issue-ms-sender",
	"PubsubTopic": "issue",
	"AllowedHosts": "*"
}
``` 
5. We should configure the password using dotnet Secret Manager tool with key as ``` DbPassword ```
6. We can now open the Visual Studio Solution and run this project under IIS Express. 
7. Next, we should import the POSTMAN collection into our POSTMAN IDE and configure the environment for the following variables. When we are done, we can run the POSTMAN test against url. We may need POSTMAN to ignore any SSL errors since we are using our own generated SSL cert.
    * TenantId
    * ClientSecret
    * ClientId
    * BaseUrl
8. We should note that running in Visual Studio in IIS Express does NOT actually publishes events over to Azure Service Bus. This is because we rely on DPAR for abstraction to Service Bus and running in IIS Express by passes DPAR. This should not impact our development because we are more interested in writing our business rules than caring about dependencies at this stage. 
9. Before starting up DPAR, we will login to the Azure Portal and create an Azure Service Bus. We should create a Listen only policy on the issue queue and note the connection string. We should create a local secrets.json file in project root and enter the following:
```
{
	"contoso-issue-ms-sender-connectionString": "<connection string>"
}
```
10. For doing end-to-end testing with DPAR, we should install DPAR if it is not yet installed and run the following under project root: ``` dapr run --app-id contoso-issue-ms --components-path .\components\ -- dotnet run ```
11. We can run the POSTMAN test again and remember to change the BaseUrl or duplicate the environment and change it so we can have both Urls for use.
12. We can run stop everything and run the following to execute the unit tests under the Test project path. ``` dotnet test ```

## Next Step
[Sprint 2: Establishing Governance practice (such as Networking, RBAC) with Azure Blueprint in Azure Subscription](../docs/SPRINT2.md)