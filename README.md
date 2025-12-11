# CodeMeet

## Getting Started

### Prerequisites

- .NET 9.0 SDK

### Configuration

#### User Secrets Setup

Navigate to the API project directory and configure the required secrets:

```bash
cd src/CodeMeet.Api
```

**Required secrets:**

```bash
# JWT Secret (required, minimum 32 characters)
dotnet user-secrets set "JwtSettings:Secret" "YourSuperSecretKeyAtLeast32Characters!"

# Admin password (required for admin user seeding)
dotnet user-secrets set "AdminSeeder:Password" "YourAdminPassword"
```

**Optional secrets (have default values):**

```bash
# Admin username (default: admin)
dotnet user-secrets set "AdminSeeder:Username" "admin"

# Admin email (default: admin@codemeet.dev)
dotnet user-secrets set "AdminSeeder:Email" "admin@codemeet.dev"
```

**Useful commands:**

```bash
# List all secrets
dotnet user-secrets list

# Remove a specific secret
dotnet user-secrets remove "JwtSettings:Secret"

# Clear all secrets
dotnet user-secrets clear
```
