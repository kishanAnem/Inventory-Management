# Retail Management System Design Document

## 1. System Architecture Overview

### 1.1 Technology Stack
- **Frontend**: Angular 16+
  - Angular Material for UI components
  - NgRx for state management
  - PWA support for offline capabilities
  - Responsive design for mobile compatibility

- **Backend**: ASP.NET Core Minimal API
  - .NET 8.0
  - Entity Framework Core
  - Identity Server for authentication
  - SignalR for real-time updates

- **Database**: SQL Server
- **Caching**: Redis
- **Message Queue**: RabbitMQ (for WhatsApp integration)

### 1.2 System Components
- Web Application (Angular SPA)
- REST API (.NET Core)
- Background Services
  - Subscription Management Service
  - WhatsApp Integration Service
  - Report Generation Service
- Authentication Service (Identity Server)

## 2. Database Design

### 2.1 Core Entities

Tenant
- TenantId (PK, uniqueidentifier)
- ShopName (nvarchar(100))
- Logo (varbinary(MAX))
- Address (nvarchar(500))
- CellNumber (varchar(20))
- SubscriptionType (varchar(50))
- SubscriptionStatus (varchar(20))
- CreatedAt (datetime2)
- ValidUntil (datetime2)

User
- UserId (PK, uniqueidentifier)
- Username (nvarchar(100))
- Email (nvarchar(256))
- PasswordHash (nvarchar(MAX))
- Mobile (int)
- Salt (nvarchar(MAX))
- Role (varchar(50))
- CreatedAt (datetime2)
- LastLogin (datetime2)
- Status (varchar(20))
- TenantId (FK, uniqueidentifier)

Product
- ProductId (PK, uniqueidentifier)
- SKU (varchar(50))
- Name (nvarchar(200))
- Description (nvarchar(500))
- CategoryTypeId (FK, uniqueidentifier)
- UnitPrice (decimal(18,2))
- MRP (decimal(18,2))
- MinStockLevel (int)
- Barcode (varchar(100))
- TenantId (FK, uniqueidentifier)
- Status (varchar(20))
- CreatedAt (datetime2)
- UpdatedAt (datetime2)

Inventory
- InventoryId (PK, uniqueidentifier)
- ProductId (FK, uniqueidentifier)
- Quantity (int)
- BatchNumber (varchar(50))
- ExpiryDate (datetime2)
- LastStockUpdate (datetime2)
- TenantId (FK, uniqueidentifier)

Customer
- CustomerId (PK, uniqueidentifier)
- Name (nvarchar(100))
- Gender (nvarchar(20))
- DOB (datetime)
- Email (nvarchar(256))
- Phone (varchar(20))
- Address (nvarchar(500))
- City (nvarchar(100))
- State (nvarchar(100))
- TenantId (FK, uniqueidentifier)
- CreatedAt (datetime2)

CategoryType
- CategoryTypeId (PK, uniqueidentifier)
- Name (nvarchar(100))
- Description (nvarchar(500))
- Status (varchar(20))
- TenantId (FK, uniqueidentifier)
- CreatedAt (datetime2)
- UpdatedAt (datetime2)

ExpenditureType
- ExpenditureTypeId (PK, uniqueidentifier)
- Name (nvarchar(100))
- Description (nvarchar(500))
- Status (varchar(20))
- TenantId (FK, uniqueidentifier)
- CreatedAt (datetime2)
- UpdatedAt (datetime2)

### 2.2 Transaction Entities

Sale
- SaleId (PK, uniqueidentifier)
- InvoiceNumber (varchar(50))
- CustomerId (FK, uniqueidentifier)
- UserId (FK, uniqueidentifier)
- SaleDate (datetime2)
- TotalAmount (decimal(18,2))
- DiscountAmount (decimal(18,2))
- AmountPaid (decimal(18,2))
- PaymentStatus (varchar(50))
- PaymentMethod (varchar(50))
- TenantId (FK, uniqueidentifier)
- CreatedAt (datetime2)
- UpdatedAt (datetime2)

SaleItem
- SaleItemId (PK, uniqueidentifier)
- SaleId (FK, uniqueidentifier)
- ProductId (FK, uniqueidentifier)
- Quantity (int)
- UnitPrice (decimal(18,2))
- Discount (decimal(18,2))
- SubTotal (decimal(18,2))
- CreatedAt (datetime2)

Expenditure
- ExpenditureId (PK, uniqueidentifier)
- Amount (decimal(18,2))
- ExpenditureTypeId (FK, uniqueidentifier)
- Description (nvarchar(500))
- Date (datetime2)
- TenantId (FK, uniqueidentifier)
- CreatedAt (datetime2)
- UpdatedBy (uniqueidentifier)

### 2.3 Subscription and Payment

PaymentTransaction
- TransactionId (PK, uniqueidentifier)
- SaleId (FK, uniqueidentifier, nullable)
- SubscriptionId (FK, uniqueidentifier, nullable)
- Amount (decimal(18,2))
- PaymentMethod (varchar(50))
- Status (varchar(50))
- TransactionDate (datetime2)
- Currency (varchar(10))
- ConversionRate (decimal(18,6))
- GatewayTransactionId (varchar(100))
- TenantId (FK, uniqueidentifier)
- CreatedAt (datetime2)

Subscription
- SubscriptionId (PK, uniqueidentifier)
- TenantId (FK, uniqueidentifier)
- PlanType (varchar(50))
- StartDate (datetime2)
- EndDate (datetime2)
- Amount (decimal(18,2))
- Status (varchar(50))
- LastPaymentDate (datetime2)
- NextPaymentDate (datetime2)
- AutoRenewal (bit)
- CreatedAt (datetime2)
- UpdatedAt (datetime2)

### 2.4 Audit and Tracking

InventoryTransaction
- TransactionId (PK, uniqueidentifier)
- ProductId (FK, uniqueidentifier)
- TransactionType (varchar(20))
- Quantity (int)
- ReferenceId (uniqueidentifier)
- ReferenceType (varchar(50))
- Date (datetime2)
- UserId (FK, uniqueidentifier)
- TenantId (FK, uniqueidentifier)
- CreatedAt (datetime2)

AuditLog
- LogId (PK, uniqueidentifier)
- EntityType (varchar(100))
- EntityId (uniqueidentifier)
- Action (varchar(50))
- ChangedBy (uniqueidentifier)
- ChangedAt (datetime2)
- OldValue (nvarchar(MAX))
- NewValue (nvarchar(MAX))
- TenantId (FK, uniqueidentifier)

### 2.5 Constraints and Indices

#### Primary Keys
- All Id columns are Primary Keys

#### Foreign Keys
- CASCADE on UPDATE
- RESTRICT on DELETE
- TenantId relationships across all tables

#### Unique Constraints
- User.Email per TenantId
- Product.SKU per TenantId
- Sale.InvoiceNumber per TenantId

#### Indices
- Customer.Phone
- Sale.SaleDate
- Sale.InvoiceNumber
- InventoryTransaction.Date
- AuditLog.ChangedAt
- Product.Barcode
- PaymentTransaction.GatewayTransactionId

## 3. API Design

### 3.1 Authentication Endpoints
```http
POST /api/auth/login
POST /api/auth/google-login
POST /api/auth/refresh-token
POST /api/auth/logout
```

### 3.2 Inventory Management
```http
GET /api/products
POST /api/products
PUT /api/products/{id}
DELETE /api/products/{id}
GET /api/products/barcode/{code}
POST /api/products/bulk-upload
```

### 3.3 Sales Management
```http
POST /api/sales
GET /api/sales
GET /api/sales/{id}
GET /api/sales/customer/{customerId}
POST /api/sales/{id}/print
```

### 3.4 Customer Management
```http
GET /api/customers
POST /api/customers
PUT /api/customers/{id}
DELETE /api/customers/{id}
POST /api/customers/{id}/whatsapp
```

### 3.5 Reports
```http
GET /api/reports/pl
GET /api/reports/sales-trend
GET /api/reports/inventory
```

### 3.6 Subscription Management
```http
GET /api/subscription
POST /api/subscription/renew
POST /api/subscription/cancel
```

## 4. Security Implementation

### 4.1 Authentication
- OAuth 2.0 with OpenID Connect
- JWT tokens with short expiration
- Refresh token rotation
- Google Sign-in integration
- MFA support

### 4.2 Authorization
- Role-based access control (RBAC)
- Tenant isolation
- Resource-based authorization
- API scope-based access

### 4.3 Data Protection
- TLS 1.3 enforcement
- Data encryption at rest
- Secure key management
- PII data protection

### 4.4 API Security
- Rate limiting
- Request validation
- CORS policy
- Security headers
- API versioning

## 5. Integration Details

### 5.1 Payment Gateway
- Razorpay integration
- Webhook handling
- Payment status synchronization
- Auto-renewal implementation

### 5.2 WhatsApp Business API
- Message templates
- Order notifications
- Payment reminders
- Promotional messages

### 5.3 Barcode System
- Barcode generation (Code128)
- Label printing integration
- Scanner integration
- Bulk barcode operations

## 6. Technical Specifications

### 6.1 Performance Requirements
- Page load time < 2 seconds
- API response time < 500ms
- Support for 100 concurrent users per tenant
- 99.9% uptime

### 6.2 Scalability
- Horizontal scaling support
- Multi-region deployment ready
- Caching strategy
- Database partitioning

### 6.3 Monitoring
- Application insights integration
- Error logging and tracking
- Performance monitoring
- Usage analytics

### 6.4 Deployment
- Docker containerization
- CI/CD pipeline
- Blue-green deployment
- Database migration strategy
