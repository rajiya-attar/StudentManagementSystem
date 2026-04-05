# Student Management System

A complete, production-ready Student Management System built with ASP.NET Core Web API, featuring JWT authentication, layered architecture, Entity Framework Core, and a React frontend.

## 📋 Table of Contents

- [Features](#features)
- [Technologies](#technologies)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [API Documentation](#api-documentation)
- [Authentication](#authentication)
- [Docker Deployment](#docker-deployment)
- [Testing](#testing)
- [Frontend](#frontend)

## 🚀 Features

- **Student Management**: Create, Read, Update, Delete (CRUD) operations for students
- **JWT Authentication**: Secure API endpoints with JWT tokens
- **Layered Architecture**: Clean separation of concerns with Controllers, Services, and Repositories
- **Global Exception Handling**: Centralized error handling with custom error responses
- **API Documentation**: Swagger/OpenAPI integration with JWT support
- **Database**: SQL Server with Entity Framework Core (Code First approach)
- **Unit Testing**: xUnit test project with Moq for mocking
- **Docker Support**: Complete Docker setup with docker-compose
- **React Frontend**: Modern React SPA for UI

## 🛠️ Technologies

- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: SQL Server
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer tokens
- **Documentation**: Swagger/OpenAPI
- **Testing**: xUnit, Moq
- **Logging**: Built-in .NET Logger / Serilog
- **Frontend**: React 18, React Router, Axios
- **Containerization**: Docker, Docker Compose

## 📁 Project Structure

```
StudentManagementSystem/
├── StudentManagementSystem.sln
├── StudentManagementSystem.API/          # Web API Layer
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── StudentsController.cs
│   ├── Middleware/
│   │   └── GlobalExceptionHandlingMiddleware.cs
│   ├── Extensions/
│   │   ├── SwaggerExtensions.cs
│   │   ├── JwtExtensions.cs
│   │   └── ServiceExtensions.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.Production.json
│   └── Program.cs
├── StudentManagementSystem.Core/         # Core Layer (Models, DTOs, Interfaces)
│   ├── DTOs/
│   │   ├── StudentDto.cs
│   │   ├── AuthDto.cs
│   │   ├── ApiResponse.cs
│   │   └── ErrorResponse.cs
│   ├── Models/
│   │   └── Student.cs
│   └── Interfaces/
│       ├── IStudentRepository.cs
│       ├── IAuthService.cs
│       ├── IJwtService.cs
│       └── ILoggerService.cs
├── StudentManagementSystem.Infrastructure/  # Infrastructure Layer
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/
│   │   ├── StudentRepository.cs
│   │   └── UserRepository.cs
│   └── Services/
│       ├── StudentService.cs
│       ├── AuthService.cs
│       └── JwtService.cs
├── StudentManagementSystem.Tests/        # Unit Tests
│   └── Services/
│       ├── StudentServiceTests.cs
│       ├── AuthServiceTests.cs
│       └── JwtServiceTests.cs
└── studentmanagement-frontend/          # React Frontend
    ├── public/
    ├── src/
    │   ├── components/
    │   │   ├── Auth/
    │   │   │   ├── Login.js
    │   │   │   └── Register.js
    │   │   ├── Students/
    │   │   │   ├── StudentList.js
    │   │   │   └── StudentForm.js
    │   │   └── Layout/
    │   │       ├── Navbar.js
    │   │       └── Navbar.css
    │   ├── App.js
    │   └── index.js
    ├── package.json
    └── Dockerfile
├── Dockerfile
└── docker-compose.yml
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Node.js 18+ (for frontend)
- Docker (optional, for containerized deployment)

### Installation

1. **Clone the repository**:
```bash
git clone <repository-url>
cd StudentManagementSystem
```

2. **Update Database Connection String**:
Edit `StudentManagementSystem.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=StudentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

3. **Update JWT Secret** (for production):
Change the `SecretKey` in `appsettings.json` to a secure random string.

4. **Restore NuGet Packages**:
```bash
dotnet restore
```

5. **Run Database Migrations**:
```bash
cd StudentManagementSystem.API
dotnet ef migrations add InitialCreate --project ../StudentManagementSystem.Infrastructure
dotnet ef database update --project ../StudentManagementSystem.Infrastructure
```

6. **Run the API**:
```bash
dotnet run
```

The API will be available at `https://localhost:7001` or `http://localhost:5000`

## 🗄️ Database Setup

### Option 1: Using LocalDB (Default)
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Option 2: Using SQL Server Express
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=StudentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Option 3: Using SQL Server with credentials
```json
"DefaultConnection": "Server=localhost;Database=StudentManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

### Migration Commands

```bash
# Create a new migration
dotnet ef migrations add MigrationName --project StudentManagementSystem.Infrastructure --startup-project StudentManagementSystem.API

# Update database
dotnet ef database update --project StudentManagementSystem.Infrastructure --startup-project StudentManagementSystem.API

# Remove last migration
dotnet ef migrations remove --project StudentManagementSystem.Infrastructure --startup-project StudentManagementSystem.API
```

## 📚 API Documentation

### Swagger UI
Once the API is running, access Swagger UI at:
```
https://localhost:7001/swagger
```

### API Endpoints

#### Authentication (Public)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and get JWT token |

#### Students (Protected - Requires JWT)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/students` | Get all students |
| GET | `/api/students/{id}` | Get student by ID |
| POST | `/api/students` | Create new student |
| PUT | `/api/students/{id}` | Update student |
| DELETE | `/api/students/{id}` | Delete student |

### API Response Format
```json
{
  "success": true,
  "message": "Students retrieved successfully",
  "data": [...],
  "statusCode": 200
}
```

## 🔐 Authentication

### Getting a JWT Token

1. **Register a new user**:
```bash
curl -X POST "https://localhost:7001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123",
    "confirmPassword": "password123",
    "fullName": "John Doe"
  }'
```

2. **Login to get token**:
```bash
curl -X POST "https://localhost:7001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'
```

Response:
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "expiration": "2024-01-15T10:30:00Z"
  }
}
```

3. **Use the token in requests**:
```bash
curl -X GET "https://localhost:7001/api/students" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..."
```

### Swagger Authentication
1. Click the **Authorize** button in Swagger UI
2. Enter: `Bearer your-jwt-token-here`
3. Click **Authorize**
4. All protected endpoints will now include the token

## 🐳 Docker Deployment

### Using Docker Compose (Recommended)

1. **Build and run**:
```bash
docker-compose up -d
```

2. **Access the API**:
```
http://localhost:8080
```

3. **Access Swagger**:
```
http://localhost:8080/swagger
```

4. **Stop services**:
```bash
docker-compose down
```

### Manual Docker Build

```bash
# Build the image
docker build -t studentmanagement-api .

# Run the container
docker run -d -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Server=your-db-server;..." \
  -e "JwtSettings__SecretKey=your-secret-key" \
  studentmanagement-api
```

## 🧪 Testing

### Run Unit Tests

```bash
cd StudentManagementSystem.Tests
dotnet test
```

### Run with verbose output
```bash
dotnet test --verbosity normal
```

### Test Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 💻 Frontend

### Setup React Frontend

1. **Install dependencies**:
```bash
cd studentmanagement-frontend
npm install
```

2. **Start development server**:
```bash
npm start
```

The frontend will be available at `http://localhost:3000`

3. **Build for production**:
```bash
npm run build
```

### Frontend Docker Build

```bash
cd studentmanagement-frontend
docker build -t studentmanagement-frontend .
docker run -d -p 80:80 studentmanagement-frontend
```

## 📊 API Testing Examples

### Using curl

**Create a student**:
```bash
curl -X POST "https://localhost:7001/api/students" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "name": "Alice Johnson",
    "email": "alice@example.com",
    "age": 22,
    "course": "Computer Science"
  }'
```

**Get all students**:
```bash
curl -X GET "https://localhost:7001/api/students" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Update a student**:
```bash
curl -X PUT "https://localhost:7001/api/students/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "name": "Alice Smith",
    "email": "alice.smith@example.com",
    "age": 23,
    "course": "Software Engineering"
  }'
```

**Delete a student**:
```bash
curl -X DELETE "https://localhost:7001/api/students/1" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Using Postman

1. Import the provided Postman collection (if available)
2. Or create requests manually:
   - Set method (GET, POST, PUT, DELETE)
   - Enter URL (e.g., `https://localhost:7001/api/students`)
   - For protected endpoints: Add header `Authorization: Bearer {token}`
   - For POST/PUT: Set Body to `raw` and `JSON`, enter JSON payload

## 🔧 Configuration

### JWT Settings
Located in `appsettings.json`:
```json
"JwtSettings": {
  "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration2024!@#$%^&*()_+",
  "Issuer": "StudentManagementSystem",
  "Audience": "StudentManagementAPI",
  "ExpirationMinutes": 60
}
```

### Logging
Default logging configuration in `appsettings.json`:
```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

## 📈 Production Checklist

- [ ] Change JWT Secret Key to a strong, random value
- [ ] Update database connection string for production
- [ ] Enable HTTPS redirection
- [ ] Configure CORS for specific origins
- [ ] Set up proper logging (Serilog with file sinks)
- [ ] Configure health checks
- [ ] Set up monitoring and alerts
- [ ] Enable rate limiting
- [ ] Configure data protection keys
- [ ] Review and harden security settings

## 🆘 Troubleshooting

### Database Connection Issues
1. Verify SQL Server is running
2. Check connection string in appsettings.json
3. Ensure TrustServerCertificate=True for local development
4. Try using (localdb)\mssqllocaldb for LocalDB

### JWT Token Issues
1. Verify SecretKey is at least 32 characters
2. Check token hasn't expired
3. Ensure "Bearer " prefix is included in Authorization header
4. Verify system time is correct

### Migration Issues
```bash
# Drop and recreate database
dotnet ef database drop --force
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 📄 License

This project is licensed under the MIT License.

## 👥 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📧 Support

For support, email support@studentmanagement.com or open an issue in the repository.
