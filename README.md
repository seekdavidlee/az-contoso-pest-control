# Introduction
Contoso Pest Control is pest control company that has been in business for over 25 years and prides itself on using natural products for most of the treatments to protect the health of its customers and environment. Contoso Pest Control serves major metropolitan areas in the US.

## Background
About 10 years ago, the company started a website which is hosted in a Co-Located Data Center where:
* Potential customers can review service offerings online.
* New customers can book appointments with sales consultants to make a home visit to understand their needs and review their service offerings. 
* Existing customers can create an account and make monthly payments with their Credit Card or Bank Account.
* Customer will need to make a call to their Customer Service department to:
    * Report issues and schedule technicians to come over.
    * Renew their annual subscription. This can also be done via mail where Contoso Pest Control will send a mail with instructions to their Billing department.
    * Address any billing concerns.
* The sales consultant uses a tablet to showcase service offerings from the website but uses a paper document to sign up customers which is scanned in the office and manually entered their CRM system.
## Customer Ask
The IT Team has been tasked with the digital transformation of the business and taking advantage of the Cloud and what it has to offer. The IT team plans to enhance the existing website as well as creating a new mobile solution. The IT Team plans to leverage Microservice Architecture when building out any new services.

The ability to report issues by customers is identified as the highest priority item. We have been contracted to build the first API microservice for customers to report issues and another scheduling microservice for a technician to be assigned based on availability. The IT team will leverage the issue Microservice which will be leveraged by their existing website so customers can create issue when logged into their account. When the Mobile solution is built, they will leverage this same Microservice so customer can create issues there as well.

# Microservice Architecture
After discussion internally, this is our proposed architecture which speaks to some of the best practices for hosting microservices on the cloud.

1. We will ensure the microservice is stateless in nature which means avoiding authentication cookies and other stateful implementations. For example, in concrete terms, the use of OAuth Bearer token will ensure our microservice remains stateless as each HTTP request would carry that token from a security perspective.
2. We will host a Swagger endpooint for the purpose of exposing documentation around its API. This will allow any team members who is interested in consuming the API to understand the specifications.
3. We will host a health check endpoint and this will allow host services to ensure the microservice is healthy or drop it and create a new instance. This will also ensure host services with capabilities to report issues to trigger any alerts.
4. We will use RESTful conventions which means POST to create, PUT to update, GET to get resources with specific routes and DELETE to remove a resource.
5. We will attempt to use a [sidecar pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/sidecar) to access dependencies so that we can abstract ourselves away from concrete implementation details. The only exception is the database access because we expect our ORM (Object-Relational-Mapping) library to take care of the dependency abstraction. This point speaks to the Pub/Sub service layer which will leverage this pattern.

![Architecture](/src/ContosoPestControl.Issue/Architecture/Contoso-Pest-Control-Microservice.png)

## Implementation
The IT Team has decided to leverage Azure as its Cloud provider and this means we will be heavily invested in key Microsoft techologies. As contractors for the Architecture and Solution, we are also expected to do the initial development work but the IT Team has a few developers who will need to maintain it. This means we need to ensure a smooth Development experience as well. 

1. We will leverage [Docker containers](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/container-docker-introduction/docker-defined) in order to provide the best out-of-box development experience for the IT Team's developers. 
2. We will be using .NET 6 for building the microservice as there is [Long-Term-Support](https://dotnet.microsoft.com/en-us/platform/support/policy) for it.
    * We will use [dotnet Secret Manager tool](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#how-the-secret-manager-tool-works) for managing any secrets such as the database password. As a best practice, we will NEVER check in any secrets so this will be perfect.     
3. We will be using MS SQL as the backend database. As such, we have documented the following command so the IT Team Devs can have a SQL Server instance running on their local dev environment.
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
9. Testing ensures our code remains reliable with any new changes. For testing:
    * We will use MS Test for writing unit tests to ensure our business logic does not break.
    * We will use POSTMAN for local end-to-end tests to ensure dependencies continue to work as dependencies themselves may have breaking changes that we may not be aware of.
