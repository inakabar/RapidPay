# RapidPay

Code test used to simulate a API for card management, including a Fee Management over card payments

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features

- Identity Server: Allows a safe and stable way to protect the api, it can even be used to authenticate a web client application a it´s acccess to the API
          - Obs: It runs on the .Net 6 instead of the .Net 7 cause there is a minor bug on the Method ToEntity related to the Automapper implementation, causing it not to be able to run the Seed Data
- Exception Handler: Centrelized Handler to format the response for exceptions on the application
- Health Check: An Endpoint to View if the external dependencies are UP, such as DataBase and Identity Server
- Entity Framework: ORM Used to keep the context on the database data.
- Unit Tests: Unit Tests using the XUnity Library, it allows the verify if the cenarios are working correctly
- Postman Collection: For the Methods for the API and the Identity Server

## Installation

Requires: Visual Studio 2022, SDK .Net 6.0 and 7.0, Sql Server Instance.

Run the commands on the Package Manager Console:

1. Set the IdentityServer Project as the Start UP project, and make sure to be in the same level as his folder, run the commands:
    - EntityFrameworkCore\Update-Database -Context PersistedGrantDbContext
    - EntityFrameworkCore\Update-Database -Context ConfigurationGrantDbContext
    - EntityFrameworkCore\Update-Database -Context AspNetIdentityDbContext
   
    - cd Auth
    - dotnet run IdentityServer/bin/Debug/net6.0/IdentityServer /seed --project IdentityServer

1. Set the RapidPay.Api Project as the Start UP project and point on Package Manager Console to the DataBase Project, and make sure to be in the same level as his folder, run the commands:
    - EntityFrameworkCore\Update-Database -Context RapidPayDbContext
    
## Usage

The Postman Collections Contain all the basics for the usage

## Contact

linkedin.com/in/igor-nakabar-9b719415a
