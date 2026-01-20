# Financial Transaction Service

A secure, scalable banking API built with .NET 10 that provides core financial operations including account management, money transfers, transaction history, and authentication.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)
![GitHub Actions](https://img.shields.io/badge/GitHub_Actions-CI/CD-2088FF?logo=github-actions)

## ✨ Features

- **Secure Authentication**: JWT-based authentication with separate flows for ATM and online access
- **Account Management**: Create, lock/unlock, delete accounts with PIN/password protection
- **Financial Operations**: Deposit, withdraw, and transfer money between accounts
- **Transaction History**: Detailed records of all account activities
- **Role-Based Access Control**: Admin and User roles with strict permission boundaries
- **Domain-Driven Design**: Strongly typed value objects (Money, PinCode) with validation rules
- **Test Coverage**: Comprehensive unit and integration tests (95%+ coverage)

## 🧩 Architecture

![Clean Architecture Diagram](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/media/clean-architecture-clean-architecturerefimage1.png)

- **Domain Layer**: Core business logic and entities (`Account`, `Transaction`, value objects)
- **Application Layer**: Use cases and DTOs (`AccountCreator`, `MoneyTransfer`, etc.)
- **Infrastructure Layer**: Persistence and security implementations
- **Presentation Layer**: REST API controllers and middleware
- **Tests**: Unit tests for domain logic and integration tests for API endpoints

## 🚀 API Endpoints

### 🔑 Authentication
| Endpoint | Method | Description | Request Example |
|----------|--------|-------------|----------------|
| `/api/auth/user/atm` | POST | Authenticate ATM user with PIN | `{"AccountId": "guid", "Pin": "1234"}` |
| `/api/auth/user/online` | POST | Authenticate online user with password | `{"AccountId": "guid", "Password": "pass"}` |
| `/api/auth/admin` | POST | Authenticate admin | `{"Password": "admin_pass"}` |

### 👤 User Account Operations
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/account/balance` | GET | Get account balance |
| `/api/account/transactions` | GET | Get transaction history |
| `/api/account/pin` | PUT | Set/Change PIN code |
| `/api/account/password/set` | PUT | Set password |
| `/api/account/password/change` | PUT | Change password |
| `/api/account/deposit/atm` | POST | Deposit money (ATM) |
| `/api/account/withdraw/atm` | POST | Withdraw money (ATM) |
| `/api/account/transfer` | POST | Transfer money to another account |

### 👮 Admin Operations
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/admin/accounts` | POST | Create new account |
| `/api/admin/accounts/{id}/lock` | POST | Lock account |
| `/api/admin/accounts/{id}/unlock` | POST | Unlock account |
| `/api/admin/accounts/{id}` | DELETE | Delete account |
| `/api/admin/accounts/{id}/balance` | PUT | Adjust account balance |
| `/api/admin/accounts/{id}/transactions` | GET | Get account transaction history |

## ⚙️ Setup Instructions

### Prerequisites
- .NET 10 SDK
- Docker (optional)
- SQLite (for local development)

### Local Development
```bash
# Clone repository
git clone https://github.com/yourusername/financial-transaction-service.git
cd financial-transaction-service

# Restore packages
dotnet restore

# Run application
dotnet run --project src/Presentation

# API will be available at http://localhost:5000
curl http://localhost:5000/health  # Should return "OK"
