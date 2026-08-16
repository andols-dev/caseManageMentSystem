# Case Management System
A web-based case management system built with ASP.NET Core MVC, C#, Entity Framework Core and SQL Server.
The purpose of the application is to provide a structured way for organizations to manage clients, cases, case managers and case history in one place.
The project is developed as a portfolio project with a focus on business application development, role-based access control, data modeling and maintainable application architecture.

## Overview
The system is designed around three main user roles:
    • Administrator – manages users, roles and system administration 
    • Case Manager – manages assigned cases and communicates case-related information 
    • Client – can access and follow their own cases 
The application is built using ASP.NET Core MVC and uses Entity Framework Core for data access.

## Features
Case Management
    • Create and manage cases 
    • Assign cases to case managers 
    • Associate cases with clients 
    • Case status management 
    • Case numbers 
    • Case notes 
    • Case history 
    • Track when cases are created and updated 
Client Management
    • Create and manage clients 
    • Search for clients 
    • View client-related cases 
    • Client profiles 
User Management
    • User registration and authentication 
    • Login/logout 
    • Role-based authorization 
    • User management 
    • Role management 
    • User profiles 
Case History
Changes to cases can be recorded in a separate history.
The history contains information such as:
    • Case 
    • User who made the change 
    • Change type 
    • Previous value 
    • New value 
    • Timestamp 
This makes it possible to follow the lifecycle of a case and see how it has changed over time.
Administration
Administrators have access to functionality for managing:
    • Users 
    • Roles 
    • Clients 
    • Case managers 
    • Cases 

## Technology Stack

## Technology           Usage
```text
C#                      Application development
ASP.NET Core MVC        Web application framework
.NET 10                 Runtime / framework
Entity Framework Core   ORM / data access
SQL Server              Database
ASP.NET Core Identity   Authentication and authorization
Razor Views             Server-side UI
Tailwind CSS            Styling
JavaScript              Client-side functionality
Git / GitHub            Version control


## Application Structure
The application uses ASP.NET Core MVC Areas to separate functionality based on user roles and responsibilities.
```text
caseManageMentSystem/
│
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/
│   │   ├── Views/
│   │   └── ...
│   │
│   ├── CaseManager/
│   │   ├── Controllers/
│   │   ├── Views/
│   │   └── ...
│   │
│   └── Client/
│       ├── Controllers/
│       ├── Views/
│       └── ...
│
├── Data/
│
├── Models/
│
├── ViewModels/
│
├── Views/
│
├── wwwroot/
│
├── Program.cs
└── appsettings.json
The Areas help keep functionality separated and make the application easier to maintain as the system grows.

## Domain Model
The core of the application is centered around cases.
A simplified representation of the domain is:
```text
ApplicationUser
      │
      ├── Client
      │
      └── Case Manager
             │
             ▼
            Case
           /    \
          /      \
       Notes   CaseHistory
A Case contains information about the client, assigned case manager, status, description and timestamps.
CaseHistory provides an audit trail for changes made to a case.

Authentication & Authorization
The application uses ASP.NET Core Identity for authentication and authorization.
Different functionality is available depending on the user's role.
```text
Administrator
    │
    ├── User management
    ├── Role management
    ├── Client management
    └── Case management

Case Manager
    │
    ├── View cases
    ├── Manage cases
    ├── Add notes
    └── Follow case history

Client
    │
    └── View own cases
Role-based authorization is used to restrict access to administrative and case-management functionality.

Database
The application uses SQL Server with Entity Framework Core.
The main entities include:
    • ApplicationUser 
    • Case 
    • Client 
    • CaseManager 
    • CaseHistory 
    • Note 
Entity Framework Core is responsible for mapping the application's domain models to the relational database.

Running the Application
Prerequisites
You need:
    • .NET 10 SDK 
    • SQL Server 
    • Git 
Clone the repository
git clone https://github.com/andols-dev/caseManageMentSystem.git
cd caseManageMentSystem
Configure the database
Update the connection string in:
appsettings.json
Example:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CaseManagement;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
Run the application
dotnet restore
dotnet build
dotnet run
The application will then be available through the local ASP.NET Core development server.

## Development Status
The project is actively being developed.
Current development focuses on improving the core case-management functionality and moving the application towards a more production-oriented architecture.
Planned improvements
    • Improve case workflow and status management 
    • Add file attachments 
    • Add notifications 
    • Add case priorities and deadlines 
    • Improve dashboard and statistics 
    • Add automated tests 
    • Improve logging and error handling 
    • Introduce additional service-layer abstractions where appropriate 
    • Improve database migration and seed-data handling 
    • Deploy the application to a cloud environment 

## What I Wanted to Learn
This project is primarily a practical exercise in building a realistic business application rather than a simple CRUD application.
The project has given me the opportunity to work with:
    • ASP.NET Core MVC 
    • Entity Framework Core 
    • SQL Server 
    • ASP.NET Core Identity 
    • Role-based authorization 
    • Relational data modeling 
    • MVC Areas 
    • ViewModels 
    • Business workflows 
    • Audit/history tracking 
    • Responsive web interfaces 
    • Git and GitHub 
A particular focus is placed on understanding how different parts of a business application interact and how the application can remain maintainable as functionality grows.

## Project Goals
The long-term goal is to develop the application into a more complete case-management platform while continuously improving its architecture, security, usability and testability.
The project is also part of my portfolio as I pursue opportunities as a system developer / web developer specializing in .NET and web-based business applications.

## Author
Andreas
GitHub:
https://github.com/andols-dev
