# ChatServer

My first C# training project — a real-time chat server built with ASP.NET Core.

## About

This is a learning project to get familiar with C# and .NET ecosystem.
Built a chat server from scratch with REST API, WebSockets, JWT authentication and PostgreSQL.

## Tech Stack

- **C# / ASP.NET Core** — server framework
- **PostgreSQL** — database
- **Entity Framework Core** — ORM
- **JWT** — authentication
- **WebSockets** — real-time messaging

## Architecture

Domain-Driven Design (DDD) with the following layers:

    ChatServer/
    ├── Domain/          # Business logic, entities, repository interfaces
    ├── Application/     # Use cases (RegisterUser, LoginUser, SendMessage)
    ├── Infrastructure/  # PostgreSQL, JWT implementation
    └── API/             # Controllers, WebSocket handler

## Getting Started

### Prerequisites
- .NET 10
- PostgreSQL

### Setup

1. Clone the repository
2. Create a `.env` file in the root:

    DB_CONNECTION=Host=localhost;Database=chatdb;Username=postgres;Password=yourpassword
    JWT_SECRET=your-secret-key-at-least-32-chars
    JWT_EXPIRES_IN_DAYS=7

3. Apply migrations:

    dotnet ef database update

4. Run the server:

    dotnet watch run

## API

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and get JWT token |

### Chat
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/chat/ws?token=JWT` | Connect to WebSocket |

## WebSocket

After connecting, send messages in this format:

    { "Text": "Hello, World!" }

Server broadcasts to all connected clients:

    {
      "Id": 1,
      "Username": "testuser",
      "Text": "Hello, World!",
      "SentAt": "2026-04-14T00:00:00"
    }