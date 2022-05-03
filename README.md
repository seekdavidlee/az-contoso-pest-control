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

# Content
Working closely with the Contoso IT team, we have agreed on splitting the work into several sprints with each sprint focused on a specific area. 

## Sprints: 
* [Sprint 1: Designing and implementing Microservice for the Cloud + Local Dev Setup](docs/SPRINT1.md)
* [Sprint 2: Establishing Governance practice (such as Networking, RBAC) with Azure Blueprint in Azure Subscription](docs/SPRINT2.md)
* [Sprint 3: Setting up DevOps CI (Continuous Integration) with Unit Testing and pushing container builds into Azure Container Registry (ACR) with GitHub Actions](docs/SPRINT3.md)
* [Sprint 4: Setting up DevOps Part 1 CD (Continuous Environment Deployment) to an Azure Subscription with GitHub Actions to create the Azure environment such as AKS Cluster](docs/SPRINT4.md)
* [Sprint 5: Setting up DevOps Part 2 CD (Continuous Code Deployment) to deploy microservice to Azure Kubernetes Service (AKS) cluster with GitHub Actions](docs/SPRINT5.md)