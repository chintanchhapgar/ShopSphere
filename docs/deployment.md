# Deployment

ShopSphere is designed for cloud-native deployment using modern DevOps practices. The application can be deployed locally, on a Virtual Machine, Azure App Service, Docker containers, or Kubernetes with minimal configuration changes.

---

# Deployment Architecture

```mermaid
flowchart TD

Developer

↓

GitHub

↓

GitHub Actions

↓

Build & Test

↓

Publish Artifact

↓

Deploy

↓

Production Server

↓

SQL Server

↓

Redis

↓

Hangfire
```

---

# Production Architecture

```mermaid
flowchart LR

Client

↓

Nginx / IIS

↓

ShopSphere API

↓

SQL Server

ShopSphere API --> Redis

ShopSphere API --> Hangfire

Hangfire --> SQL Server
```

---

# Deployment Options

Supported deployment targets:

- IIS
- Docker
- Azure App Service
- Azure Container Apps
- Azure Kubernetes Service (AKS)
- Linux VM
- Windows Server

---

# Environment Configuration

Configuration is managed through **appsettings** and environment variables.

Example:

```text
appsettings.json

appsettings.Development.json

appsettings.Production.json
```

---

# Required Configuration

## Database

```json
ConnectionStrings
{
  "DefaultConnection": ""
}
```

---

## Redis

```json
ConnectionStrings
{
  "Redis": ""
}
```

---

## JWT

```json
Jwt
{
    "Issuer": "",
    "Audience": "",
    "SecretKey": ""
}
```

---

## Email

```json
EmailSettings
{
    "Host": "",
    "Port": "",
    "Username": "",
    "Password": ""
}
```

---

# Deployment Pipeline

```mermaid
flowchart LR

Restore

↓

Build

↓

Run Tests

↓

Publish

↓

Deploy
```

---

# GitHub Actions

Current pipeline performs:

- Restore packages
- Build solution
- Run unit tests
- Run integration tests
- Generate code coverage
- Upload artifacts

---

# Publish

Generate deployment files:

```bash
dotnet publish src/ShopSphere.Api \
-c Release \
-o publish
```

Output:

```text
publish/

ShopSphere.Api.dll

appsettings.json

wwwroot/

Dependencies
```

---

# Database Migration

Before starting the application:

```bash
dotnet ef database update
```

Or automatically during startup:

```csharp
dbContext.Database.Migrate();
```

---

# Hangfire

Hangfire automatically:

- Creates database schema
- Starts processing server
- Registers recurring jobs

Dashboard:

```text
/hangfire
```

---

# Health Checks

Available endpoints:

```text
/health

/health/live

/health/ready

/health-ui
```

---

# Logging

Production logging uses Serilog.

Outputs include:

- Console
- File
- Structured JSON
- Seq (Future)
- Azure Monitor (Future)

---

# Static Files

Static assets are served from:

```text
wwwroot/
```

---

# HTTPS

Production should always enable:

- HTTPS
- HSTS
- TLS 1.2+

---

# Reverse Proxy

Example deployment:

```mermaid
flowchart LR

Internet

↓

Nginx

↓

Kestrel

↓

ShopSphere API
```

---

# Scaling

Horizontal scaling supported.

```mermaid
flowchart LR

Load Balancer

↓

API Instance 1

API Instance 2

API Instance 3

↓

Shared SQL Server

↓

Redis
```

---

# Deployment Checklist

## Before Release

- Build succeeds
- Tests pass
- Database migration generated
- Secrets configured
- Connection strings verified
- JWT configured
- Email configured
- Redis available
- Hangfire running

---

# Production Checklist

✅ HTTPS Enabled

✅ SQL Server Available

✅ Redis Running

✅ Hangfire Running

✅ Health Checks Enabled

✅ Serilog Configured

✅ Database Migrated

---

# Monitoring

Recommended monitoring:

- Application Logs
- SQL Performance
- Redis Health
- Hangfire Dashboard
- Health Endpoints
- Application Metrics

---

# Backup Strategy

Recommended backups:

- SQL Server Daily Backup
- Weekly Full Backup
- Hourly Transaction Logs
- Configuration Backup
- Artifact Backup

---

# Disaster Recovery

Recovery process:

```text
Restore Database

↓

Deploy Latest Release

↓

Restore Configuration

↓

Verify Health Checks

↓

Resume Traffic
```

---

# Future Improvements

- Docker Support
- Docker Compose
- Kubernetes Manifests
- Helm Charts
- Azure DevOps Pipeline
- Terraform Infrastructure
- Blue-Green Deployment
- Canary Releases
- Automatic Rollback
- Secrets via Azure Key Vault
- Prometheus Metrics
- Grafana Dashboards

---

# Technologies

- ASP.NET Core 8
- SQL Server
- Redis
- Hangfire
- GitHub Actions
- Serilog
- Health Checks
- IIS / Nginx
- Docker (Planned)
- Kubernetes (Planned)