# Inventory Management System - Project Structure

## Solution Structure Overview

```
Inventory-Management/
├── src/
│   ├── InventoryManagement.API/               # API Project
│   │   ├── Controllers/                       # API endpoints
│   │   ├── Middlewares/                       # Custom middleware components
│   │   ├── Extensions/                        # Extension methods
│   │   └── appsettings.json                  # Configuration settings
│   │
│   ├── InventoryManagement.Core/             # Core Domain Project
│   │   ├── Entities/                         # Domain entities (Product, Customer, etc.)
│   │   ├── Interfaces/                       # Core interfaces
│   │   ├── Events/                          # Domain events
│   │   └── Exceptions/                       # Custom domain exceptions
│   │
│   ├── InventoryManagement.Infrastructure/   # Infrastructure Project
│   │   ├── Persistence/                      # Database related code
│   │   │   ├── Configurations/               # Entity configurations
│   │   │   ├── Repositories/                 # Data access implementations
│   │   │   └── Context/                      # DbContext and migrations
│   │   ├── Services/                         # External service implementations
│   │   ├── Identity/                         # Authentication/Authorization
│   │   └── Messaging/                        # RabbitMQ implementations
│   │
│   ├── InventoryManagement.Application/      # Application Project
│   │   ├── DTOs/                            # Data transfer objects
│   │   ├── Services/                         # Application services
│   │   ├── Validators/                       # Request validators
│   │   └── Mappings/                        # AutoMapper profiles
│   │
│   └── InventoryManagement.Shared/          # Shared Project
│       ├── Constants/                        # Shared constants
│       └── Utils/                           # Utility classes
│
├── tests/                                    # Test Projects
│   ├── InventoryManagement.UnitTests/        # Unit tests
│   ├── InventoryManagement.IntegrationTests/ # Integration tests
│   └── InventoryManagement.FunctionalTests/  # End-to-end tests
│
├── client/                                   # Angular Frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/                        # Core module
│   │   │   │   ├── guards/                  # Route guards
│   │   │   │   ├── interceptors/            # HTTP interceptors
│   │   │   │   └── services/                # Core services
│   │   │   ├── shared/                      # Shared module
│   │   │   │   ├── components/              # Reusable components
│   │   │   │   ├── directives/             # Custom directives
│   │   │   │   └── pipes/                  # Custom pipes
│   │   │   ├── features/                    # Feature modules
│   │   │   │   ├── inventory/              # Inventory management
│   │   │   │   ├── sales/                  # Sales management
│   │   │   │   ├── customers/              # Customer management
│   │   │   │   └── reports/                # Reports and analytics
│   │   │   └── auth/                       # Authentication module
│   │   ├── assets/                         # Static assets
│   │   └── environments/                    # Environment configs
│   │
│   ├── e2e/                                 # End-to-end tests
│   └── angular.json                         # Angular configuration
│
├── tools/                                    # Build and deployment scripts
│   ├── scripts/                             # Build automation scripts
│   └── templates/                           # Template files
│
├── docs/                                     # Documentation
│   ├── api/                                 # API documentation
│   └── guides/                              # User guides
│
├── .github/                                  # GitHub configurations
│   └── workflows/                           # GitHub Actions workflows
│
├── docker/                                   # Docker configurations
│   ├── api/                                 # API Dockerfile
│   ├── client/                              # Client Dockerfile
│   └── docker-compose.yml                   # Docker compose config
│
├── .gitignore                               # Git ignore file
├── README.md                                # Project overview
└── solution.sln                             # Solution file
```

## Project Components Description

### Backend (.NET 8)

1. **InventoryManagement.API**
   - Main API entry point
   - RESTful endpoints
   - API configuration
   - Middleware implementations

2. **InventoryManagement.Core**
   - Domain entities and business logic
   - Core interfaces
   - Domain events and handlers
   - Business rules and validations

3. **InventoryManagement.Infrastructure**
   - Database access implementation
   - External service integrations
   - Identity and security
   - Message queue implementation

4. **InventoryManagement.Application**
   - Application services
   - DTOs and mappings
   - Business logic orchestration
   - Input validation

5. **InventoryManagement.Shared**
   - Shared utilities
   - Common constants
   - Helper functions

### Frontend (Angular)

1. **Core Module**
   - Authentication
   - Route guards
   - HTTP interceptors
   - Core services

2. **Shared Module**
   - Reusable components
   - Common directives
   - Shared pipes
   - Common utilities

3. **Feature Modules**
   - Inventory management
   - Sales management
   - Customer management
   - Reports and analytics

### DevOps

1. **Docker Support**
   - Containerization for API
   - Containerization for client
   - Docker compose for local development

2. **CI/CD**
   - GitHub Actions workflows
   - Build and deployment scripts
   - Environment configurations

### Documentation
   - API documentation
   - User guides
   - Development guides
   - Architecture documentation