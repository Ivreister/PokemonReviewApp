# PokemonReviewApp
My first tutorial REST API project for reviewing Pokemon from scratch using a modern tech stack. The project includes:

Backend: .NET 8 + Entity Framework Core
Database: PostgreSQL with automatic migrations
Deployment: Fully configured CI/CD via Render.com

🛠 What I did

1. API development
-Designed an architecture with a clear separation of layers:
Controllers → Repositories → EF Core → PostgreSQL
-Implemented:
6 entities (Pokemon, Review, Category, etc.)
CRUD operations with validation
Many-to-Many relationships (e.g. Pokemon ↔ Category)

2. Working with data
-Wrote a Seed service to fill the database with test data
-Configured automatic migrations for Render.com

3. Deployment and infrastructure
-Dockerized: Created a Dockerfile for containerization.
-Render.com:
Deployed service with autodeployment from GitHub
Connected managed PostgreSQL
Configured environment variables (ConnectionStrings, PORT, etc.)
-Swagger: Adapted for production
