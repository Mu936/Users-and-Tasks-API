Task Management API
This API provides a robust solution for user and task management, built with ASP.NET Core 7.0. It implements JWT-based authentication and follows RESTful principles for seamless integration with client applications.

Getting Started
Prerequisites
Before running the application, ensure you have the following installed:

.NET 7.0 SDK - Download here
A code editor (Visual Studio 2022, VS Code, or JetBrains Rider)
Git version control system
Installation and Setup
To get the API up and running locally, follow these steps:

Clone the repository to your local machine:

git clone https://github.com/Mu936/Users-and-Tasks-API.git
cd Users-and-Tasks-API
Navigate to the project directory and restore the required packages:

cd UsersAndTasksAPI
dotnet restore
Set up the database by running migrations:

dotnet ef database update
Start the application:

dotnet run
Once running, you can access:

API Base URL: https://localhost:5001
Interactive API Documentation: https://localhost:5001/swagger
Authentication Workflow
User Registration
To start using the API, you'll first need to register a user account. This is a one-time setup process.

API Endpoint: POST /api/Auth/register

Example request:

curl -X POST "https://localhost:5001/api/Auth/register" \
     -H "Content-Type: application/json" \
     -d '{
           "username": "johndoe",
           "password": "SecurePass123!",
           "email": "john.doe@example.com"
         }'
Obtaining an Access Token
After registration, you'll need to authenticate to receive an access token.

API Endpoint: POST /api/Auth/login

Sample authentication request:

curl -X POST "https://localhost:5001/api/Auth/login" \
     -H "Content-Type: application/json" \
     -d '{"username":"johndoe","password":"SecurePass123!"}'
On successful authentication, you'll receive a response like this:

{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600,
    "tokenType": "Bearer",
    "username": "johndoe",
    "userId": 1
}
Making Authenticated Requests
Include the received token in the Authorization header for all protected endpoints:

Authorization: Bearer your_jwt_token_here
Example of fetching tasks:

curl -X GET "https://localhost:5001/api/Tasks" \
     -H "Authorization: Bearer your_jwt_token_here" \
     -H "Content-Type: application/json"
Testing the API
Interactive Documentation with Swagger
For an interactive way to explore the API, Swagger UI is available at https://localhost:5001/swagger after starting the application. Here's how to use it:

Open the Swagger UI in your preferred web browser
Locate the "Authorize" button in the top-right corner
Enter your JWT token in the format: Bearer your_token_here
Click "Authorize" and then "Close"
You can now test any endpoint directly from the interface
Testing with cURL
For command-line testing, here's a sample script that demonstrates the complete authentication flow:

#!/bin/bash

# Register a new user
curl -X POST "https://localhost:5001/api/Auth/register" \
     -H "Content-Type: application/json" \
     -d '{
           "username": "api_tester",
           "password": "TestPass123!",
           "email": "tester@example.com"
         }'

echo -e "\n---\n"

# Authenticate and get token
RESPONSE=$(curl -s -X POST "https://localhost:5001/api/Auth/login" \
     -H "Content-Type: application/json" \
     -d '{"username":"api_tester","password":"TestPass123!"}')

echo "Authentication Response:"
echo $RESPONSE | jq

# Extract token from response
TOKEN=$(echo $RESPONSE | jq -r '.token')

echo -e "\n---\n"
echo "Testing protected endpoint with the received token..."

# Make authenticated request
curl -X GET "https://localhost:5001/api/Tasks" \
     -H "Authorization: Bearer $TOKEN" \
     -H "Content-Type: application/json"
Postman Collection
For more comprehensive testing, you can use the included Postman collection:

Import the collection from the /postman directory
Configure the baseUrl environment variable to point to https://localhost:5001
Start with the authentication request to obtain a token
Use the token in the Authorization header for subsequent requests
Protected Endpoints
All endpoints except /api/Auth/register and /api/Auth/login require authentication. You must include the JWT token in the Authorization header as shown above.

🔍 Testing with Swagger UI
Open the Swagger UI at: https://localhost:5001/swagger
Click the "Authorize" button (top right)
Enter: Bearer your_jwt_token_here
Click "Authorize" and then "Close"
Now you can test all endpoints directly from the Swagger UI
📝 Example: Complete Workflow
Register (if not already done):

curl -X POST "https://localhost:5001/api/Auth/register" \
     -H "Content-Type: application/json" \
     -d '{"username":"reviewer","password":"Review@123","email":"reviewer@example.com"}'
Login to get token:

curl -X POST "https://localhost:5001/api/Auth/login" \
     -H "Content-Type: application/json" \
     -d '{"username":"reviewer","password":"Review@123"}'
Copy the token from the response.

Use the token to access protected endpoints:

# Get all tasks
curl -X GET "https://localhost:5001/api/Tasks" \
     -H "Authorization: Bearer your_token_here" \
     -H "Content-Type: application/json"
⚠️ Troubleshooting
403 Forbidden: Make sure you've included the Authorization header with a valid token
401 Unauthorized: Your token might have expired (default 60 minutes). Log in again to get a new token.
400 Bad Request: Check your request body and parameters
API Reference
Authentication Endpoints
Register New User
Endpoint: POST /api/Auth/register
Description: Creates a new user account
Request Body:
{
  "username": "string (required, 3-50 characters)",
  "password": "string (required, min 8 characters)",
  "email": "string (valid email format)"
}
Response: User details and authentication token
User Login
Endpoint: POST /api/Auth/login
Description: Authenticates user and returns JWT token
Request Body:
{
  "username": "string",
  "password": "string"
}
Response: Authentication token and user information
Task Management
List All Tasks
Endpoint: GET /api/Tasks
Description: Retrieves all tasks for the authenticated user
Query Parameters:
completed (boolean) - Filter by completion status
dueBefore (date) - Filter by due date (ISO format)
assignedTo (number) - Filter by assignee ID
Response: Array of task objects
Get Task by ID
Endpoint: GET /api/Tasks/{id}
Description: Retrieves a specific task by its ID
Response: Task details
Create New Task
Endpoint: POST /api/Tasks
Description: Creates a new task
Request Body:
{
  "title": "string (required)",
  "description": "string (optional)",
  "dueDate": "string (ISO date format, optional)",
  "isCompleted": "boolean (default: false)",
  "assigneeId": "number (ID of the assigned user)"
}
Update Task
Endpoint: PUT /api/Tasks/{id}
Description: Updates an existing task
Request Body: Same as create, but all fields are optional
Delete Task
Endpoint: DELETE /api/Tasks/{id}
Description: Removes a task from the system
Response: 204 No Content on success
Common Issues and Solutions
Port Conflicts
If you encounter port conflicts when running the application:

Change the application port

Locate Properties/launchSettings.json
Update the applicationUrl to use a different port (e.g., change 5001 to 5002)
Terminate the conflicting process On macOS/Linux:

lsof -i :5001  # Find process using port 5001
kill -9 <PID>  # Replace <PID> with the actual process ID
On Windows:

netstat -ano | findstr :5001
taskkill /PID <PID> /F
Database Connection Problems
Reset the database

Delete the UsersTasks.db file in the project directory
Restart the application - a new database will be created automatically
Manual database updates If you've made changes to your models, run:

cd UsersAndTasksAPI
dotnet ef database update
SSL Certificate Warnings
Trust the development certificate

dotnet dev-certs https --clean
dotnet dev-certs https --trust
Temporary workaround For development, you can disable HTTPS in Properties/launchSettings.json by:

Setting "sslPort": 0
Removing https from applicationUrl
Commenting out app.UseHttpsRedirection() in Program.cs
Authentication Issues
Token not working: Ensure you're including the Bearer  prefix in the Authorization header
Token expired: The default token expiration is 60 minutes. Get a new token by logging in again
Invalid credentials: Double-check username/password and ensure the user exists
Additional Resources
.NET Documentation
Entity Framework Core
JWT Authentication
License
This project is licensed under the MIT License - see the LICENSE file for details.

Authentication
POST /api/Auth/register - Register a new user
POST /api/Auth/login - Get JWT token
Tasks
GET /api/Tasks - Get all tasks
GET /api/Tasks/{id} - Get task by ID
POST /api/Tasks - Create a new task
PUT /api/Tasks/{id} - Update a task
DELETE /api/Tasks/{id} - Delete a task
2. Using the Access Token
Include the token in the Authorization header of your requests to protected endpoints:

Authorization: Bearer your_jwt_token_here
Example using curl:

curl -X GET "https://your-api-url/api/Users" \
     -H "Authorization: Bearer your_jwt_token_here" \
     -H "Content-Type: application/json"
Available Endpoints
Authentication
POST /api/Auth/login - Get JWT token
Users (Requires Authentication)
GET /api/Users - Get all users
GET /api/Users/{id} - Get user by ID
POST /api/Users - Create a new user
PUT /api/Users/{id} - Update a user
DELETE /api/Users/{id} - Delete a user
Tasks (Requires Authentication)
GET /api/Tasks - Get all tasks
GET /api/Tasks/{id} - Get task by ID
POST /api/Tasks - Create a new task
PUT /api/Tasks/{id} - Update a task
DELETE /api/Tasks/{id} - Delete a task
Development Setup
Clone the repository
Update the database connection string in appsettings.json if needed
Run database migrations:
dotnet ef database update
Run the application:
dotnet run
The API will be available at https://localhost:5001 or http://localhost:5000.

Default Admin User
If you're running this for the first time, you'll need to register a user first by making a POST request to /api/Users with a new user's details. The first user created will have admin privileges by default.

JWT Configuration
The JWT settings can be configured in appsettings.json:

Jwt:Key - Secret key used to sign tokens
Jwt:Issuer - Token issuer
Jwt:Audience - Token audience
Jwt:ExpiresInMinutes - Token expiration time in minutes
