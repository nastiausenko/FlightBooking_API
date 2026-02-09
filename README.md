# Flight Booking API

REST API for flight management and seat booking.

## Architecture

The project is structured using layered architecture:
- **API** – HTTP controllers and request handling
- **Application** – business logic, services, DTOs, validation
- **Domain** – core domain models and repository interfaces
- **Infrastructure** – database access, EF Core, Identity, authentication

## Key decisions
- Business logic is placed in Application services, controllers are thin
- Repositories are implemented in Infrastructure to isolate EF Core
- Database constraints are used to guarantee data integrity
- JWT authentication with role-based authorization (Admin / Passenger)

## Error handling
- Validation errors are handled via model validation
- NotFound and business exceptions are handled globally
- Database constraint violations are converted to domain-specific exceptions

## Authentication
- ASP.NET Identity is used for user management
- JWT tokens contain user id, email, username and roles

