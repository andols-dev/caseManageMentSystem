# Case Management System

A web-based case management system built with ASP.NET Core MVC, C#, Entity Framework Core and SQL Server.

## Overview

The system is designed around three main user roles:

- Administrator – manages users, roles and system administration
- Case Manager – manages assigned cases and communicates case-related information
- Client – can access and follow their own cases

The application is built using ASP.NET Core MVC and uses Entity Framework Core for data access.

## Features

### Case Management

- Create and manage cases
- Assign cases to case managers
- Associate cases with clients
- Case status management
- Case numbers
- Case notes
- Case history
- Track when cases are created and updated

### Client Management

- Create and manage clients
- Search for clients
- View client-related cases
- Client profiles

### User Management

- User registration and authentication
- Login/logout
- Role-based authorization
- User management
- Role management
- User profiles

### Case History

Changes to cases can be recorded in a separate history.
The history contains information such as:

- Case
- User who made the change
- Change type
- Previous value
- New value
- Timestamp

This makes it possible to follow the lifecycle of a case and see how it has changed over time.

### Administration

Administrators have access to functionality for managing:

- Users
- Roles
- Clients
- Case managers
- Cases

## Technology Stack

```text
Technology Stack        Technology Usage
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
```

## Application Structure

The application uses ASP.NET Core MVC Areas to separate functionality based on user roles and responsibilities.

```text
caseManageMentSystem/
│
├── Areas/
│ ├── Admin/
│ │ ├── Controllers/
│ │ ├── Views/
│ │ └── ...
│ │
│ ├── CaseManager/
│ │ ├── Controllers/
│ │ ├── Views/
│ │ └── ...
│ │
│ └── Client/
│ ├── Controllers/
│ ├── Views/
│ └── ...
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
```

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
```

A Case contains information about the client, assigned case manager, status, description and timestamps.
CaseHistory provides an audit trail for changes made to a case.

## Authentication & Authorization

The application uses ASP.NET Core Identity for authentication and authorization.
Different functionality is available depending on the user's role.
Role-based authorization is used to restrict access to administrative
and case-management functionality.

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

```

## Author

Andreas
GitHub:
https://github.com/andols-dev
