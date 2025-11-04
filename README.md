# Bank API - Sistema de Gestión Bancaria

API REST desarrollada en .NET 9 para gestión de clientes, cuentas y movimientos bancarios con validaciones de negocio críticas.

---

## 🛠️ Tecnologías

- **.NET 9.0** - Framework
- **Entity Framework Core** - ORM
- **SQL Server 2022** - Base de datos
- **Docker** - Contenedorización
- **xUnit + Moq** - Testing

---

## 🏗️ Arquitectura y Patrones

### Clean Architecture
```
API Layer          → Controllers, Middleware
Application Layer  → Services, DTOs, Interfaces
Infrastructure     → Repositories, DbContext, EF Core
Domain Layer       → Entities, Enums
```

### Patrones Implementados
- ✅ **Repository Pattern** - Abstracción de acceso a datos
- ✅ **Unit of Work** - Gestión de transacciones
- ✅ **Dependency Injection** - Inversión de control
- ✅ **DTO Pattern** - Separación de capas

---

## 🚀 Ejecución con Docker
```bash
# Clonar repositorio
git clone <url-repositorio>
cd DevsuApp.BE

# Levantar con Docker Compose
docker-compose up --build

# En modo background
docker-compose up -d --build
```

### Acceso a la Aplicación

- **Swagger UI**: http://localhost:5000/swagger
- **API**: http://localhost:5000/index.html
- **Base URL**: http://localhost:5000/api

### Credenciales SQL Server (Docker)
```
Server: localhost:1433
Usuario: sa
Contraseña: YourStrong@Password123
Base de datos: BankDB
```

### Detener servicios
```bash
docker-compose down

# Con limpieza de volúmenes
docker-compose down -v
```

---

## 📋 Endpoints Principales

| Recurso | Métodos | Ruta |
|---------|---------|------|
| **Clientes** | GET, POST, PUT, PATCH, DELETE | `/api/clientes` |
| **Cuentas** | GET, POST, PUT, PATCH, DELETE | `/api/cuentas` |
| **Movimientos** | GET, POST, DELETE | `/api/movimientos` |
| **Reportes** | GET | `/api/reportes` |

---

## 📏 Reglas de Negocio Implementadas

### 1. Gestión de Saldo
- **Créditos (depósitos)**: Valores positivos, se suman al saldo
- **Débitos (retiros)**: Valores negativos, se restan del saldo
- El saldo se persiste en cada transacción

### 2. Validación: Saldo Insuficiente
```
Si saldo = 0 y tipo = Débito
→ Error 400: "Saldo no disponible"
```

### 3. Validación: Cupo Diario Excedido
```
Límite diario: $1,000
Si débitos_del_día > $1,000
→ Error 400: "Cupo diario Excedido"
```

---

## 🗄️ Base de Datos

### Script SQL
El script de inicialización se encuentra en:
```
Scripts/BaseDatos.sql
```

---

## 🧪 Pruebas Unitarias
```bash
cd DevsuApp.BE.Tests
dotnet test
```

**Casos implementados:**
1. ✅ Creación de cliente exitosa
2. ✅ Validación de saldo insuficiente
3. ✅ Validación de cupo diario excedido

---

## 📁 Estructura del Proyecto
```
DevsuApp.BE/
├── DevsuApp.BE/
│   ├── API/Controllers/          # Endpoints REST
│   ├── Application/
│   │   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Services/             # Lógica de negocio
│   │   └── Interfaces/           # Contratos
│   ├── Domain/
│   │   ├── Entities/             # Entidades del dominio
│   │   └── Enums/                # Tipos de cuenta y movimiento
│   └── Infrastructure/
│       ├── Data/                 # DbContext
│       └── Repositories/         # Acceso a datos
├── DevsuApp.BE.Tests/            # Pruebas unitarias
├── Scripts/BaseDatos.sql         # Script de BD
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

## 📦 Colección de Postman

Los ejemplos de uso y casos de prueba están disponibles en la colección de Postman incluida en este repositorio.

---

## 👨‍💻 Autor

Ever Peña Ballesteros
Noviembre 2025
