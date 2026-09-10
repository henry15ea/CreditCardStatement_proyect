# API Documentation - Credit Card Statement

## Base URL

```
https://localhost:7001
```

## Autenticación

La API utiliza JWT Bearer Authentication. Obtener un token mediante el endpoint de login.

```
Authorization: Bearer {token}
```

## Endpoints

### Auth

#### POST /api/auth/login

Autenticar usuario y obtener token JWT.

**Request Body:**
```json
{
  "email": "henry.aq@mail.com",
  "password": "contraseña123"
}
```

**Response (200):**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login successful",
  "user": {
    "id": 1,
    "name": "Henry aquino",
    "email": "henry.aq@mail.com"
  }
}
```

**Response (401):**
```json
{
  "success": false,
  "message": "Invalid email or password"
}
```

---

### Credit Cards

#### GET /api/creditcard/cards/{cardHolderId}

Obtener todas las tarjetas de crédito de un titular.

**Path Parameters:**
- `cardHolderId` (int): ID del titular

**Headers:**
- `Authorization: Bearer {token}`

**Response (200):**
```json
[
  {
    "id": 1,
    "cardHolderId": 1,
    "cardNumber": "4532015112830366",
    "creditLimit": 5000.00,
    "currentBalance": 114.47,
    "interestRate": 25.00,
    "minimumPaymentRate": 5.00,
    "cardHolderName": "Henry Aquino"
  }
]
```

---

#### GET /api/creditcard/statement/{creditCardId}

Obtener estado de cuenta de una tarjeta de crédito.

**Path Parameters:**
- `creditCardId` (int): ID de la tarjeta

**Query Parameters:**
- `month` (int): Mes (1-12)
- `year` (int): Año

**Headers:**
- `Authorization: Bearer {token}`

**Response (200):**
```json
{
  "creditCard": {
    "id": 1,
    "cardHolderId": 1,
    "cardNumber": "4532015112830366",
    "creditLimit": 5000.00,
    "currentBalance": 114.47,
    "interestRate": 25.00,
    "minimumPaymentRate": 5.00,
    "cardHolderName": "Henry Aquino"
  },
  "totalPurchasesCurrentMonth": 114.47,
  "totalPurchasesPreviousMonth": 200.00,
  "bonifiableInterest": 28.62,
  "minimumPayment": 5.72,
  "totalToPay": 114.47,
  "cashPaymentWithInterest": 143.09,
  "availableBalance": 4885.53,
  "transactions": [
    {
      "id": 1,
      "creditCardId": 1,
      "type": 1,
      "date": "2026-09-02T00:00:00",
      "description": "Amazon Purchase",
      "amount": 45.99,
      "typeName": "Purchase"
    }
  ]
}
```

---

#### GET /api/creditcard/transactions/{creditCardId}

Obtener todas las transacciones de un mes específico.

**Path Parameters:**
- `creditCardId` (int): ID de la tarjeta

**Query Parameters:**
- `month` (int): Mes (1-12)
- `year` (int): Año

**Headers:**
- `Authorization: Bearer {token}`

**Response (200):**
```json
[
  {
    "id": 1,
    "creditCardId": 1,
    "type": 1,
    "date": "2026-09-02T00:00:00",
    "description": "Amazon Purchase",
    "amount": 45.99,
    "typeName": "Purchase"
  },
  {
    "id": 5,
    "creditCardId": 1,
    "type": 2,
    "date": "2026-09-01T00:00:00",
    "description": "Payment",
    "amount": 200.00,
    "typeName": "Payment"
  }
]
```

---

#### POST /api/creditcard/purchase

Agregar una nueva compra.

**Headers:**
- `Authorization: Bearer {token}`
- `Content-Type: application/json`

**Request Body:**
```json
{
  "creditCardId": 1,
  "date": "2026-09-04",
  "description": "Amazon Purchase",
  "amount": 45.99
}
```

**Response (201):**
```json
{
  "id": 6,
  "creditCardId": 1,
  "type": 1,
  "date": "2026-09-04T00:00:00",
  "description": "Amazon Purchase",
  "amount": 45.99,
  "typeName": "Purchase"
}
```

**Validaciones:**
- `creditCardId`: Debe ser mayor a 0
- `date`: Requerido, no puede ser futuro
- `description`: Requerido, máximo 200 caracteres
- `amount`: Debe ser mayor a 0, máximo 100,000

---

#### POST /api/creditcard/payment

Realizar un pago.

**Headers:**
- `Authorization: Bearer {token}`
- `Content-Type: application/json`

**Request Body:**
```json
{
  "creditCardId": 1,
  "date": "2026-09-04",
  "amount": 50.00
}
```

**Response (201):**
```json
{
  "id": 7,
  "creditCardId": 1,
  "type": 2,
  "date": "2026-09-04T00:00:00",
  "description": "Payment",
  "amount": 50.00,
  "typeName": "Payment"
}
```

**Validaciones:**
- `creditCardId`: Debe ser mayor a 0
- `date`: Requerido, no puede ser futuro
- `amount`: Debe ser mayor a 0, máximo 100,000

---

### Health Check

#### GET /health

Verificar estado de la aplicación y conexión a base de datos.

**Response (200):**
```
Healthy
```

**Response (503):**
```
Unhealthy
```

---

## Cálculos Financieros

### Interés Bonificable
```
Interés Bonificable = Saldo Total × Porcentaje Interés Configurable
Interés Bonificable = $114.47 × 25% = $28.62
```

### Cuota Mínima a Pagar
```
Cuota Mínima = Saldo Total × Porcentaje Configurable Saldo Mínimo
Cuota Mínima = $114.47 × 5% = $5.72
```

### Monto Total a Pagar
```
Monto Total a Pagar = Saldo Total = $114.47
```

### Pago de Contado con Intereses
```
Pago de Contado = Saldo Total + Interés Bonificable
Pago de Contado = $114.47 + $28.62 = $143.09
```

## Manejo de Errores

Todos los errores siguen un formato estándar:

```json
{
  "error": {
    "message": "Descripción del error",
    "statusCode": 400
  }
}
```

### Códigos de Estado

| Código | Descripción |
|--------|-------------|
| 200 | Éxito |
| 201 | Creado exitosamente |
| 400 | Solicitud inválida |
| 401 | No autorizado |
| 404 | No encontrado |
| 500 | Error interno del servidor |

## Swagger

La documentación Swagger está disponible en:

```
https://localhost:7001/swagger
```

## Postman Collection

Importar la colección completa desde:
```
database/postman/CreditCardStatement.postman_collection.json
```
