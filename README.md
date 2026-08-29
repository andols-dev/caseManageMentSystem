# Case Management System

A web-based case management system built with ASP.NET Core MVC, Entity Framework Core, SQL Server, and Docker.
<img width="1864" height="1056" alt="login" src="https://github.com/user-attachments/assets/9cedfc5a-12d7-4a0d-93a5-9ab31b8d8e23" />

<img width="1864" height="1056" alt="logged_out" src="https://github.com/user-attachments/assets/eab47a80-219d-474a-afcf-2ea9f9303b7b" />

<img width="1864" height="1149" alt="dashboard" src="https://github.com/user-attachments/assets/d6ec82a0-2acb-480b-ba06-6ffea96ce1c0" />

<img width="1879" height="914" alt="clientsList" src="https://github.com/user-attachments/assets/a6550207-52c9-4bbe-a507-5d3d29fbaecb" />
<img width="1879" height="914" alt="clientslist_create_case" src="https://github.com/user-attachments/assets/cd3a3ab9-8f50-41a1-b1ee-41a4205c72e1" />
<img width="1864" height="941" alt="clientslist_details" src="https://github.com/user-attachments/assets/6546ffb7-dbb4-4903-91b1-7a56b2144e6e" />

## Running the application with Docker

### Prerequisites

You only need:

- Docker Desktop
- Git

1. Clone the repository
   - git clone https://github.com/andols-dev/caseManageMentSystem.git
   - cd caseManageMentSystem
2. Start the application
   - docker compose up -d

3. Open the application
   - Open:
     http://localhost:8080
4. Stop the application
   - docker compose down
   - To remove the containers and the database volume:
     docker compose down -v
   - Use docker compose down -v only when you intentionally want to remove the local Docker database.

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
