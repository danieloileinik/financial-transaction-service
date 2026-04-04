# Financial Transaction Service — API Reference

Authentication: `Authorization: Bearer <token>` header required on all protected endpoints.

---

## Authentication

### POST /api/auth/user/atm

Authenticate via PIN (ATM flow). Returns a signed JWT.

**Request:**

```http
POST /api/auth/user/atm
Content-Type: application/json

{
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "pin": "1234"
}
```

**Response 200:**

```text
"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Response 401:**

```text
"Incorrect pin code"
```

---

### POST /api/auth/user/online

Authenticate via password (online banking flow). Returns a signed JWT.

**Request:**

```http
POST /api/auth/user/online
Content-Type: application/json
{
  "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "password": "MySecurePass123"
}
```

**Response 200:**

```text
"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Response 401:**

```text
"Incorrect password"
```

---

### POST /api/auth/admin

Authenticate as administrator. Returns a signed admin JWT.

**Request:**

```http
POST /api/auth/admin
Content-Type: application/json
{
  "password": "AdminPassword"
}
```

**Response 200:**

```text
"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Response 401:**

```text
"Invalid admin password"
```

---

## Account (User)

> All endpoints require `Authorization: Bearer <user-token>`

### GET /api/accounts/balance

Returns the current balance of the authenticated account.

**Request:**

```http
GET /api/accounts/balance
Authorization: Bearer <token>
```

**Response 200:**

```json
{
  "balance": 1500.00
}
```

**Response 404:**

```text
"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```

---

### GET /api/accounts/transactions

Returns transaction history. Optional date range filter via query params.

**Request:**

```http
GET /api/accounts/transactions?from=2024-01-01T00:00:00Z&to=2024-12-31T23:59:59Z
Authorization: Bearer <token>
```

**Response 200:**

```json
{
  "transactions": [
    {
      "type": "Deposit",
      "amount": 1000.00,
      "timestamp": "2024-06-15T10:30:00+00:00"
    },
    {
      "type": "Withdraw",
      "amount": 250.00,
      "timestamp": "2024-06-16T14:00:00+00:00"
    }
  ]
}
```

**Response 404:**

```text
"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```

---

### POST /api/accounts/deposit/atm

Deposit money into the authenticated account.

**Request:**

```http
POST /api/accounts/deposit/atm
Authorization: Bearer <token>
Content-Type: application/json
{
  "amount": 500.00
}
```

**Response 204:** No content

**Response 403:**

```text
"Account is locked"
```

**Response 400:**

```text
"The amount of money is out of range"
```

---

### POST /api/accounts/withdraw/atm

Withdraw money from the authenticated account.

**Request:**

```http
POST /api/accounts/withdraw/atm
Authorization: Bearer <token>
Content-Type: application/json
{
  "amount": 200.00
}
```

**Response 204:** No content

**Response 403:**

```text
"Insufficient funds"
```

**Response 403:**

```text
"Account is locked"
```

---

### POST /api/accounts/transfer

Transfer money to another account.

**Request:**

```http
POST /api/accounts/transfer
Authorization: Bearer <token>
Content-Type: application/json
{
  "receiverId": "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
  "amount": {
    "amount": 300.00
  }
}
```

**Response 204:** No content

**Response 404:**

```text
"Account with ID:9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d not found"
```

**Response 403:**

```text
"Insufficient funds"
```

---

### PUT /api/accounts/pin

Set or update the PIN code for the authenticated account.

**Request:**

```http
PUT /api/accounts/pin
Authorization: Bearer <token>
Content-Type: application/json
{
  "pin": "5678"
}
```

**Response 204:** No content

**Response 400:**

```text
"Invalid pin code format"
```

---

### PUT /api/accounts/password/set

Set a password for the authenticated account.

**Request:**

```http
PUT /api/accounts/password/set
Authorization: Bearer <token>
Content-Type: application/json
{
  "password": "NewPassword123"
}
```

**Response 204:** No content

---

### PUT /api/accounts/password/change

Change password by providing the current password.

**Request:**

```http
PUT /api/accounts/password/change
Authorization: Bearer <token>
Content-Type: application/json
{
  "oldPassword": "OldPassword123",
  "newPassword": "NewPassword456"
}
```

**Response 204:** No content

**Response 401:**

```text
"Incorrect password"
```

---

## Admin

> All endpoints require `Authorization: Bearer <admin-token>`

### POST /api/admin/accounts

Create a new bank account. PIN and password are auto-generated server-side.

**Request:**

```http
POST /api/admin/accounts
Authorization: Bearer <admin-token>
```

**Response 200:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

### POST /api/admin/accounts/{id}/lock

Lock an account, preventing all deposits and withdrawals.

**Request:**

```http
POST /api/admin/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa6/lock
Authorization: Bearer <admin-token>
```

**Response 204:** No content

**Response 404:**

```text
"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```

---

### POST /api/admin/accounts/{id}/unlock

Unlock a previously locked account.

**Request:**

```http
POST /api/admin/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa6/unlock
Authorization: Bearer <admin-token>
```

**Response 204:** No content

**Response 404:**

```text
"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```

---

### DELETE /api/admin/accounts/{id}

Soft-delete an account. Hidden from all queries but retained in the database.

**Request:**

```http
DELETE /api/admin/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer <admin-token>
```

**Response 204:** No content

**Response 404:**

```text
"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```

---

### PUT /api/admin/accounts/{id}/balance

Adjust account balance. Positive = deposit, negative = withdrawal.

**Request:**

```http
PUT /api/admin/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa6/balance
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "amount": -200.00
}
```

**Response 204:** No content

**Response 403:**

```text
"Insufficient funds"
```

**Response 404:**

```text
"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```

---

### GET /api/admin/accounts/{id}/transactions

View transaction history for any account. Supports optional date range filter.

**Request:**

```http
GET /api/admin/accounts/3fa85f64-5717-4562-b3fc-2c963f66afa6/transactions?from=2024-01-01T00:00:00Z&to=2024-12-31T23:59:59Z
Authorization: Bearer <admin-token>
```

**Response 200:**

```json
{
  "transactions": [
    {
      "type": "Deposit",
      "amount": 1000.00,
      "timestamp": "2024-06-15T10:30:00+00:00"
    }
  ]
}
```

**Response 404:**

```text
"Account with ID:3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
```

---

## Health Check

### GET /health

No authentication required.

**Request:**

```http
GET /health
```

**Response 200:**

```text
OK
```

---

## Error Reference

| Status | Meaning                                          |
|--------|--------------------------------------------------|
| 200    | OK — request succeeded, body contains result     |
| 204    | No Content — request succeeded, no body          |
| 400    | Bad Request — invalid input                      |
| 401    | Unauthorized — wrong password or PIN             |
| 403    | Forbidden — account locked or insufficient funds |
| 404    | Not Found — account does not exist               |
| 500    | Internal Server Error                            |
