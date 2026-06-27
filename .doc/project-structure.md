[Back to README](../README.md)

## Project Structure

The project should be structured as follows:

```
root
├── src/
├── tests/
└── README.md
```

### Layers (DDD)

The solution under `src/` is organized into four explicit layers, also reflected
as solution folders in `Ambev.DeveloperEvaluation.sln`:

* **Domain** (`Ambev.DeveloperEvaluation.Domain`) — entities, value objects, domain
  events, repository interfaces and business rules. Has no dependency on any
  other layer.
* **Application** (`Ambev.DeveloperEvaluation.Application`) — use cases (CQRS
  commands/queries and their handlers), orchestrating the domain to fulfill a
  request. Depends only on Domain.
* **Infrastructure** (`Ambev.DeveloperEvaluation.ORM`, `Ambev.DeveloperEvaluation.Common`,
  `Ambev.DeveloperEvaluation.IoC`) — persistence (EF Core/PostgreSQL, MongoDB),
  cross-cutting concerns (security, validation, logging) and the composition
  root that wires everything together. Implements the interfaces defined in
  Domain.
* **Presentation** (`Ambev.DeveloperEvaluation.WebApi`) — the ASP.NET Core host:
  controllers, request/response models and API-level validation. Depends on
  Application (via MediatR) and is composed by Infrastructure's IoC.

`tests/` mirrors this with Unit, Integration and Functional test projects.
