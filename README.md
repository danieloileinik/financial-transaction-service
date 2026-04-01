# Financial Transaction Service

A robust financial transaction management system built with .NET, following Domain-Driven Design (DDD) and Clean Architecture principles.

## Architecture

The project follows Clean Architecture with the following layers:

- **Domain**: Core business logic, entities, value objects, and domain errors
- **Application**: Use cases, DTOs, and application services
- **Infrastructure**: Data persistence, external services, and implementations
- **Presentation**: API controllers and request/response handling

## Technologies

- .NET 10
- Entity Framework Core 8
- PostgreSQL with Npgsql
- JWT Authentication
- BCrypt for password hashing
- FluentAssertions for testing
- Testcontainers for integration tests
- xUnit for unit and integration testing

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker (for running PostgreSQL in development)
- PostgreSQL 15+ (if running locally)

### Configuration

Create an `appsettings.json` file with the following structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=financialdb;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "Issuer": "FinancialTransactionService",
    "Audience": "FinancialTransactionService",
    "SecretKey": "your-256-bit-secret-key-minimum-32-characters",
    "ExpirationMinutes": 60
  },
  "AdminPassword": "your-secure-admin-password"
}
```
Running the Application
bash

# Restore dependencies
dotnet restore

# Run migrations
dotnet ef database update --project src/FinancialTransactionService.Infrastructure --startup-project src/FinancialTransactionService.Presentation

# Run the application
dotnet run --project src/FinancialTransactionService.Presentation

Running Tests
bash

dotnet test


User ATM Authentication

Authenticates a user via ATM interface using account ID and PIN code.

Endpoint: POST /api/auth/user/atm

Request Body:
```json

{
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "pin": "1234"
}
```
Response (200 OK):
```text

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjNmYTg1ZjY0LTU3MTctNDU2Mi1iM2ZjLTJjOTYzZjY2YWZhNiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MDAwMDAwMDB9.signature
```
Response (401 Unauthorized):
```json

"Incorrect pin code"
```
Response (400 Bad Request):
```json

"Invalid pin code format"
```
User Online Authentication

Authenticates a user via online interface using account ID and password.

Endpoint: POST /api/auth/user/online

Request Body:
```json

{
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "password": "userpassword123"
}
```
Response (200 OK):
```text

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjNmYTg1ZjY0LTU3MTctNDU2Mi1iM2ZjLTJjOTYzZjY2YWZhNiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MDAwMDAwMDB9.signature
```
Response (401 Unauthorized):
```text
"Incorrect password"
```
Admin Authentication

Authenticates as an administrator.

Endpoint: POST /api/auth/admin

Request Body:
```json

{
  "password": "admin-password"
}
```
Response (200 OK):
```text

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTcwMDAwMDAwMH0.signature
```
Response (401 Unauthorized):
```json

"Invalid admin password"
```
Response (400 Bad Request):
```json

"Request body is required"
Account Operations
```
All endpoints in this section require a valid user JWT token. The token must include the account ID in the claims.
Get Balance

Retrieves the current balance of the authenticated account.

Endpoint: GET /api/accounts/balance

Headers:
```text

Authorization: Bearer {user_jwt_token}
```
Response (200 OK):
```json

{
  "balance": 1250.50
}
```
Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Get Transaction History

Retrieves transaction history for the authenticated account.

Endpoint: GET /api/accounts/transactions

Query Parameters:

    From (optional): Start date and time (ISO 8601 format)

    To (optional): End date and time (ISO 8601 format)

Headers:
```text

Authorization: Bearer {user_jwt_token}
```
Request Example:
```text

GET /api/accounts/transactions?From=2024-01-01T00:00:00Z&To=2024-12-31T23:59:59Z
```
Response (200 OK):
```json

{
  "transactions": [
    {
      "type": "Deposit",
      "amount": 500.00,
      "timestamp": "2024-01-15T10:30:00Z"
    },
    {
      "type": "Withdraw",
      "amount": 100.00,
      "timestamp": "2024-01-16T14:20:00Z"
    }
  ]
}
```
Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Set PIN Code

Sets or updates the PIN code for the account.

Endpoint: PUT /api/accounts/pin

Headers:
```text

Authorization: Bearer {user_jwt_token}
```
Request Body:
```json

{
  "pin": "1234"
}
```
Response (204 No Content): (No response body)

Response (400 Bad Request):
```json

"Invalid pin code format"

Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Set Password

Sets the initial password for the account.

Endpoint: PUT /api/accounts/password/set

Headers:
```text

Authorization: Bearer {user_jwt_token}
```
Request Body:
```json

{
  "password": "newpassword123"
}
```
Response (204 No Content): (No response body)

Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
````
Change Password

Changes the account password.

Endpoint: PUT /api/accounts/password/change

Headers:
```text

Authorization: Bearer {user_jwt_token}
```
Request Body:
```json

{
  "oldPassword": "oldpassword123",
  "newPassword": "newpassword456"
}
```
Response (204 No Content): (No response body)

Response (401 Unauthorized):
```json

"Incorrect password"
```
Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Deposit Money

Deposits money into the account.

Endpoint: POST /api/accounts/deposit/atm

Headers:
```text

Authorization: Bearer {user_jwt_token}
```
Request Body:
```json

{
  "amount": 500.00
}
```
Response (204 No Content): (No response body)

Response (400 Bad Request):
```json

"The amount of money is out of range"
```
Response (403 Forbidden):
```json

"Account is locked"
```
Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Withdraw Money

Withdraws money from the account.

Endpoint: POST /api/accounts/withdraw/atm
```
Headers:
```text

Authorization: Bearer {user_jwt_token}
```
Request Body:
```json

{
  "amount": 100.00
}
```
Response (204 No Content): (No response body)

Response (400 Bad Request):
```json

"The amount of money is out of range"
```
Response (403 Forbidden):
```json

"Account is locked"
```
Response (403 Forbidden):
```json

"Insufficient funds"

Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Transfer Money

Transfers money to another account.

Endpoint: POST /api/accounts/transfer

Headers:
```text

Authorization: Bearer {user_jwt_token}
```
Request Body:
```json

{
  "receiverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": {
    "amount": 250.00
  }
}
```
Response (204 No Content): (No response body)

Response (400 Bad Request):
```json

"The amount of money is out of range"
```
Response (403 Forbidden):
```json

"Insufficient funds"
```
Response (403 Forbidden):
```json

"Account is locked"
```
Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Admin Operations

All endpoints in this section require an admin JWT token with the "Admin" role.
Create Account

Creates a new account with automatically generated PIN and password.

Endpoint: POST /api/admin/accounts

Headers:
```text

Authorization: Bearer {admin_jwt_token}
```
Response (200 OK):
```json

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```
Lock Account

Locks an account, preventing any transactions.

Endpoint: POST /api/admin/accounts/{id}/lock

Headers:
```text

Authorization: Bearer {admin_jwt_token}
```
Path Parameters:

    id: Account UUID
Response (204 No Content): (No response body)

Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Unlock Account

Unlocks a locked account.

Endpoint: POST /api/admin/accounts/{id}/unlock

Headers:
```text

Authorization: Bearer {admin_jwt_token}
```
Path Parameters:

    id: Account UUID

Response (204 No Content): (No response body)

Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Delete Account

Soft deletes an account.

Endpoint: DELETE /api/admin/accounts/{id}

Headers:
```text

Authorization: Bearer {admin_jwt_token}
```
Path Parameters:

    id: Account UUID

Response (204 No Content): (No response body)

Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Adjust Balance

Adjusts account balance (positive for deposit, negative for withdrawal).

Endpoint: PUT /api/admin/accounts/{id}/balance

Headers:
```text

Authorization: Bearer {admin_jwt_token}
```
Path Parameters:

    id: Account UUID

Request Body:
```json

{
  "amount": 1000.00
}
```
Response (204 No Content): (No response body)

Response (400 Bad Request):
```json

"The amount of money is out of range"
```
Response (403 Forbidden):
```json

"Account is locked"
```
Response (403 Forbidden):
```json

"Insufficient funds"
```
Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Get Account Transactions

Retrieves transaction history for any account.

Endpoint: GET /api/admin/accounts/{id}/transactions

Headers:
```text

Authorization: Bearer {admin_jwt_token}
```
Path Parameters:

    id: Account UUID

Query Parameters:

    From (optional): Start date and time (ISO 8601 format)

    To (optional): End date and time (ISO 8601 format)

Request Example:
```text

GET /api/admin/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa6/transactions?From=2024-01-01T00:00:00Z&To=2024-12-31T23:59:59Z
```
Response (200 OK):
```json

{
  "transactions": [
    {
      "type": "Deposit",
      "amount": 500.00,
      "timestamp": "2024-01-15T10:30:00Z"
    },
    {
      "type": "Withdraw",
      "amount": 100.00,
      "timestamp": "2024-01-16T14:20:00Z"
    }
  ]
}
```

Response (404 Not Found):
```json

"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```
Testing

The project includes comprehensive unit and integration tests:
Unit Tests

Located in FinancialTransactionService.Tests/Unit/, these tests cover domain logic without external dependencies.
Integration Tests

Located in FinancialTransactionService.Tests/Integration/, these tests use Testcontainers to spin up a real PostgreSQL database.

To run tests:
bash

dotnet test

Database Schema
Accounts Table
Column	Type	Description
Id	UUID	Primary key
Balance	DECIMAL(18,2)	Current account balance
IsLocked	BOOLEAN	Account lock status
IsDeleted	BOOLEAN	Soft delete flag
PasswordHash	```text	Hashed password (optional)
PinCode	VARCHAR(4)	PIN code (optional)
Transactions Table
Column	Type	Description
Id	UUID	Primary key
AccountId	UUID	Foreign key to Accounts
Amount	DECIMAL(18,2)	Transaction amount
Timestamp	TIMESTAMPTZ	Transaction timestamp (UTC)
TransactionType	VARCHAR(13)	Discriminator: "Deposit" or "Withdraw"
Indexes

    IX_Transactions_AccountId: For filtering transactions by account

    IX_Transactions_AccountId_Timestamp: For efficient date-range queries

Error Handling

The API uses the ErrorOr library and returns appropriate HTTP status codes:

    200 OK: Successful operation with response body

    204 No Content: Successful operation without response body

    400 Bad Request: Validation errors

    401 Unauthorized: Authentication required or invalid credentials

    403 Forbidden: Insufficient permissions (locked account, insufficient funds)

    404 Not Found: Resource not found

    500 Internal Server Error: Unexpected server error


