
# Online Store Project

This repository contains the source code for an online store, built using .NET 9.0, showcasing a robust backend implementation that includes MSSQL for database management, Entity Framework for object-relational mapping, JWT for authentication, and Swagger for API documentation.

## Technologies Used

- **.NET 9.0**: The latest release of .NET for building high-performance, cross-platform applications.
- **MSSQL**: A relational database management system used to store and manage the application's data.
- **Entity Framework Core**: An object-relational mapper that enables .NET developers to work with a database using .NET objects.
- **JWT Authentication**: Implements JSON Web Tokens for secure authentication.
- **Swagger**: Used to build interactive API documentation and help with the frontend and backend integration.
- **ASP.NET Core Web API**: Framework for building RESTful applications.

## Architecture

This project follows the Controller-Service-Context pattern:
- **Controllers**: Handle incoming HTTP requests and send responses.
- **Services**: Contain business logic and talk to data access layers.
- **Data Context (DbContext)**: Responsible for database operations under Entity Framework.

## Features

- **User Authentication and Authorization**
  - JWT Authentication with roles to securely manage user access.
  - Refresh Tokens to maintain user sessions safely.
- **Product Management**
  - CRUD operations for product inventory.

## Getting Started

### Prerequisites

- .NET SDK 9.0
- SQL Server 2019 or later

### Setting Up the Development Environment

1. **Clone the repository**
   ```bash
   git clone https://github.com/SerbanTiurbe21/APP-PROJECT.git
   cd APP-PROJECT
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Setup the database**
   - Make sure SQL Server is running.
   - Update the connection string in `appsettings.json`.
   - Run the following command to apply migrations to the database:
     ```bash
     dotnet ef database update
     ```

4. **Run the application**
   ```bash
   dotnet run
   ```

### Using Swagger

Navigate to `http://localhost:<port>/swagger` to view the Swagger UI and interact with the API.

## Testing

For unit and integration testing, refer to the **[NET-Testing repository](https://github.com/SerbanTiurbe21/NET-Testing)**. This repository contains various test projects designed to validate the functionality of the Online Store application, ensuring that business logic, API endpoints, and data access layers perform as expected.


## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

Serban Tiurbe - serbantiurbe@gmail.com

Project Link: https://github.com/SerbanTiurbe21/APP-PROJECT
