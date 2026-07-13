# Inventory

The Inventory module is responsible for managing product stock levels, tracking inventory movements, and ensuring products are available for purchase. It integrates closely with the Catalog and Orders modules to provide accurate stock validation and reservation.

---

# Features

- Stock Management
- Inventory Tracking
- Stock Reservation
- Stock Release
- Inventory Transactions
- Automatic Stock Updates
- Low Stock Detection (Planned)
- CQRS Architecture
- Repository Pattern
- Entity Framework Core

---

# Module Overview

```mermaid
flowchart TD

A[Client]

A --> B[Products API]
A --> C[Orders API]

B --> D[MediatR]
C --> D

D --> E[Commands / Queries]

E --> F[Handlers]

F --> G[Inventory Repository]

G --> H[Entity Framework Core]

H --> I[(SQL Server)]
```

---

# Inventory Architecture

```mermaid
flowchart LR

Product --> Inventory

Inventory --> InventoryTransactions

Orders --> Inventory

Inventory --> SQLServer
```

---

# Inventory Entity

Each product has a single inventory record.

| Property | Description |
|----------|-------------|
| Id | Inventory identifier |
| ProductId | Associated product |
| Quantity | Available stock |
| ReservedQuantity | Reserved stock |
| CreatedOn | Audit field |
| ModifiedOn | Audit field |

---

# Inventory Transactions

Every inventory change is recorded.

Supported transaction types:

- Stock In
- Stock Out
- Reservation
- Reservation Release
- Order Completed
- Order Cancelled
- Manual Adjustment

---

## Inventory Transaction Entity

| Property | Description |
|----------|-------------|
| Id | Transaction identifier |
| InventoryId | Inventory reference |
| Quantity | Quantity changed |
| Type | Transaction type |
| Reason | Description |
| CreatedOn | Audit timestamp |

---

# Inventory Lifecycle

```mermaid
stateDiagram-v2

[*] --> Available

Available --> Reserved : Customer Checkout

Reserved --> Sold : Payment Successful

Reserved --> Available : Order Cancelled

Sold --> Completed

Completed --> [*]
```

---

# Stock Reservation Flow

```mermaid
sequenceDiagram

Customer->>Order API: Create Order

Order API->>Inventory: Reserve Stock

Inventory->>Database: Update Reserved Quantity

Database-->>Inventory: Success

Inventory-->>Order API: Stock Reserved

Order API-->>Customer: Order Created
```

---

# Order Completion Flow

```mermaid
sequenceDiagram

Payment->>Inventory: Deduct Stock

Inventory->>Database: Reduce Quantity

Database-->>Inventory: Saved

Inventory->>InventoryTransactions: Add Transaction

InventoryTransactions->>Database: Save

Database-->>Inventory: Success
```

---

# Reservation Logic

Available Stock is calculated as:

```
Available = Quantity - ReservedQuantity
```

Example:

| Quantity | Reserved | Available |
|----------|-----------|------------|
| 100 | 20 | 80 |
| 50 | 10 | 40 |
| 15 | 15 | 0 |

---

# Inventory Validation

Before creating an order, the system verifies:

- Product exists
- Product is active
- Inventory exists
- Available quantity is sufficient

If validation fails:

- Order creation is rejected
- Stock remains unchanged

---

# CQRS Commands

Current inventory operations are implemented using MediatR.

Examples:

- CreateInventoryCommand
- UpdateInventoryCommand
- ReserveInventoryCommand
- ReleaseInventoryCommand
- DeductInventoryCommand

---

# Queries

Examples:

- GetInventoryByProductIdQuery
- GetInventoryQuery
- GetInventoryTransactionsQuery

---

# Entity Relationships

```mermaid
erDiagram

PRODUCT ||--|| INVENTORY : owns

INVENTORY ||--o{ INVENTORY_TRANSACTION : records

ORDER ||--o{ ORDER_ITEM : contains

ORDER_ITEM }o--|| PRODUCT : references
```

---

# Repository Layer

The Inventory module follows the Repository Pattern.

Responsibilities include:

- Retrieve inventory
- Update stock
- Reserve stock
- Release stock
- Persist inventory transactions

---

# Inventory Rules

Current business rules include:

- Stock cannot become negative
- Reserved quantity cannot exceed available quantity
- Every inventory modification creates a transaction record
- Inventory updates occur within a database transaction

---

# Error Scenarios

Examples:

| Error | Description |
|--------|-------------|
| PRODUCT_NOT_FOUND | Product does not exist |
| INVENTORY_NOT_FOUND | Inventory record missing |
| INSUFFICIENT_STOCK | Not enough inventory |
| INVALID_QUANTITY | Quantity must be greater than zero |

---

# Integration with Orders

```mermaid
flowchart LR

Customer

Customer --> Order

Order --> Inventory

Inventory --> Database

Order --> Payment

Payment --> Inventory

Inventory --> TransactionHistory
```

---

# Inventory Workflow

```mermaid
flowchart TD

Create Product

↓

Create Inventory

↓

Stock Available

↓

Customer Places Order

↓

Reserve Stock

↓

Payment Successful

↓

Deduct Stock

↓

Record Inventory Transaction
```

---

# Current Capabilities

✅ Inventory per Product

✅ Stock Tracking

✅ Inventory Transactions

✅ Stock Reservation

✅ Stock Deduction

✅ Order Integration

✅ Entity Framework Persistence

✅ CQRS

✅ Repository Pattern

---

# Planned Enhancements

Future improvements include:

- Low Stock Notifications
- Warehouse Management
- Multi-Warehouse Inventory
- Inventory Transfers
- Barcode Support
- Batch/Lot Tracking
- Expiry Date Management
- Supplier Restocking
- Inventory Dashboard
- Inventory Reports
- Automatic Reordering
- Stock Forecasting

---

# Technologies

- ASP.NET Core 8
- Entity Framework Core
- SQL Server
- MediatR
- Clean Architecture
- Repository Pattern
- CQRS
- FluentValidation
- Serilog