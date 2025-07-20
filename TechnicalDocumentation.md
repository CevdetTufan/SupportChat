# SupportChat Technical Documentation

## Overview

**SupportChat** is a multi-layered chat application designed to manage customer support processes efficiently. The project follows modern software architecture principles, ensuring scalability, maintainability, and clear separation of concerns.

---

## Architecture

The solution is organized into the following main layers:

- **API Layer** (`SupportChat.Api`): Handles HTTP requests, defines endpoints, and manages communication with external clients.
- **Application Layer** (`SupportChat.Application`): Contains business logic, command/query handlers, and orchestrates workflows.
- **Domain Layer** (`SupportChat.Domain`): Defines core business entities, domain events, and business rules.
- **Infrastructure Layer** (`SupportChat.Infrastructure`): Implements data access, integrations, and background workers.
- **Testing** (`SupportChat.Test`): Contains unit and integration tests for the system.

---

## Layer Responsibilities

### 1. API Layer (`SupportChat.Api`)
- **Program.cs**: Application entry point, service and middleware configuration.
- **Endpoints/**: HTTP endpoints for chat-related operations.
    curl http://localhost:7000/chat/createSession \
    request POST
- **Middlewares/**: Global middlewares such as error handling.

### 2. Application Layer (`SupportChat.Application`)
- **Commands/**: Write operations (e.g., create, assign, end sessions) and their handlers.
- **Queries/**: Read operations and their handlers.
- **Interfaces/**: Abstractions for command/query handlers, coordination, and persistence.
- **Modules/**: IoC container module definitions for dependency injection.

### 3. Domain Layer (`SupportChat.Domain`)
- **Agents/**: Agent entity, events (e.g., shift started/completed), and related logic.
- **ChatSessions/**: Chat session entity, events (e.g., session created/timed out), and exceptions.
- **Business Rules**: Core domain logic and invariants.

### 4. Infrastructure Layer (`SupportChat.Infrastructure`)
- **Coordination/**: Agent selection and session assignment strategies.
- **Data/**: Entity Framework-based data access and Unit of Work implementation.
- **Repositories/**: Concrete repository implementations for data access.
- **Workers/**: Background workers (e.g., chat coordination, polling monitor).

### 5. Testing (`SupportChat.Test`)
- **Domains/**: Unit tests for domain logic.
- **Integrations/**: Integration tests across layers.

---

## Main Workflows

### 1. Starting a New Chat Session
- A user initiates a chat via the API, triggering a `CreateChatSessionCommand`.
- The Application Layer processes the command, creates a new chat session, and emits relevant domain events.

### 2. Assigning a Session to an Agent
- The system uses coordination services to select an available agent based on defined strategies (e.g., seniority, availability).
- The session is assigned, and the agent is notified.

---

## Technologies Used

- **.NET (C#)**: Core application framework.
- **Entity Framework Core**: ORM for data access.
- **Docker**: Containerization for deployment and development.
- **xUnit/NUnit**: Testing frameworks.
- **AutoFac**: DI

---

## Background Services (Workers)

The Infrastructure layer includes background services that run independently of the main API process. These are implemented as hosted services (workers) and are responsible for handling asynchronous or periodic tasks. Key background services include:

- **ChatCoordinatorWorker**: Responsible for coordinating chat session assignments, monitoring session states, and ensuring that sessions are assigned to available agents according to the business rules and assignment strategies.
- **PollingMonitorWorker**: Monitors the state of chat sessions and agents, performing periodic checks to update statuses, handle timeouts, or trigger other automated actions as required by the business logic.

These workers are registered with the application's dependency injection container and run in the background as long as the application is running. They help offload long-running or scheduled tasks from the main request/response pipeline, improving scalability and responsiveness.

---

## Configuration & Deployment

- **Docker**: Use `docker/docker-compose.yml` to spin up all services as containers.
- **appsettings.json**: Application configuration (database, etc.).
- **Migrations**: Database migrations are located under `SupportChat.Infrastructure/Migrations`.

---

## Extensibility & Customization

- **Adding New Commands/Queries**: Extend the Application Layer with new command/query handlers.
- **Custom Assignment Strategies**: Implement the `ISessionAssignmentStrategy` interface for new agent assignment logic.

---

## Testing

- **Unit Tests**: Validate domain logic and business rules.
- **Integration Tests**: Ensure correct interaction between layers and with external systems.
- Tests are organized under `test/SupportChat.Test`.

---

## File Structure (Key Directories)

```
SupportChat/
  docker/                  # Docker configuration
  src/
    SupportChat.Api/       # API Layer
    SupportChat.Application/ # Application Layer
    SupportChat.Domain/    # Domain Layer
    SupportChat.Infrastructure/ # Infrastructure Layer
  test/
    SupportChat.Test/      # Tests
```

---

## Example: Adding a New Feature

1. **Define Domain Logic**: Add new entities/events in `SupportChat.Domain` if needed.
2. **Create Command/Query**: Implement command/query and handler in `SupportChat.Application`.
3. **Expose via API**: Add new endpoint in `SupportChat.Api`.
4. **Persist Data**: Update repositories and data models in `SupportChat.Infrastructure`.
5. **Test**: Add unit/integration tests in `SupportChat.Test`.

---

## Team & Shift Structure

**Team & Shift Structure,** reflecting that the Overflow pool only operates during the 08:00–
16:00 day shift (night coverage is handled exclusively by Team C):

| Team Name    | Shift Hours (Local Time) | Members                                | Base Efficiency Factors | Max Concurrent Chats (per agent) | Team Capacity (concurrent) |
|--------------|--------------------------|----------------------------------------|-------------------------|----------------------------------|-----------------------------|
| **Team A**   | 08:00 – 16:00            | 1 × Team Lead (0.5), 2 × Mid (0.6), 1 × Junior (0.4) | Lead: 0.5<br>Mid: 0.6<br>Jnr: 0.4 | 10                              | ⌊(1×10×0.5)+(2×10×0.6)+(1×10×0.4)⌋ = ⌊5+12+4⌋ = **21** |
| **Team B**   | 08:00 – 16:00            | 1 × Senior (0.8), 1 × Mid (0.6), 2 × Junior (0.4)    | Snr: 0.8<br>Mid: 0.6<br>Jnr: 0.4 | 10                              | ⌊(1×10×0.8)+(1×10×0.6)+(2×10×0.4)⌋ = ⌊8+6+8⌋ = **22** |
| **Team C**   | 16:00 – 00:00            | 2 × Mid (0.6)                                   | Mid: 0.6                      | 10                              | ⌊(2×10×0.6)⌋ = ⌊12⌋ = **12**     |
| **Overflow** | Office hours only        | 6 × Junior (0.4)                                 | Jnr: 0.4                      | 10                              | ⌊(6×10×0.4)⌋ = ⌊24⌋ = **24**     |

### Capacity & Queue Limits

- **Per-Team Capacity**  
  Calculated as:  capacity = ⌊∑(agents × 10 chats × efficiency)⌋

- **Maximum Queue Length**  
1.5 × team capacity (rounded down).  
- e.g. Team A queue limit = ⌊1.5 × 21⌋ = **31**  
- Team C queue limit = ⌊1.5 × 12⌋ = **18**  

Overflow only activates when Team A or Team B reaches its queue limit between 08:00–16:00.
Outside of those hours (16:00–08:00), Team C covers the night shift without Overflow.

---

**Note:** All times use the server’s local zone (Istanbul Time, UTC+3).
