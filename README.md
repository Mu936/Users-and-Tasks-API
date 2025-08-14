# Users-and-Tasks API

A simple RESTful API built with C# and ASP.NET Core that manages users and tasks. This API allows you to create, read, update, and delete users and tasks, and includes JWT authentication for secure access.

## Features

- **Manage Users**:
  - Create, read, update, delete users
- **Manage Tasks**:
  - Create, read, update, delete tasks
  - Filter tasks by user, date, active/expired
- **JWT Authentication**:
  - Secure endpoints with token-based authentication
- **SQLite Database**:
  - Lightweight, file-based storage for data persistence

## Technologies Used

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQLite
- JWT (JSON Web Tokens)

## Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/Mu936/Users-and-Tasks-API.git

*Navigate to the project folder:

cd UsersAndTasksAPI



*Restore dependencies:

dotnet restore


*Build the project:

dotnet build


*Run the API:

dotnet run


*Access the API documentation (Swagger) at:

https://localhost:<port>/swagger
*Notes

Make sure the database file (UsersTasks.db) is in the correct location or update the connection string in appsettings.json.

All endpoints except login require a valid JWT token for access.

Author

Masithembi Mulalo
Email: masithembimulaloivy@gmail.com
