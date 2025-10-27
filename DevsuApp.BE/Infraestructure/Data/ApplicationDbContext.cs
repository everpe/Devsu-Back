using DevsuApp.BE.Domain.Entities;
using DevsuApp.BE.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevsuApp.BE.Infraestructure.Data;

 public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        // DbSets
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de herencia TPH (Table Per Hierarchy)
            modelBuilder.Entity<Cliente>()
                .ToTable("Clientes");

            // No creamos tabla para Persona porque es abstracta/base
            // EF Core usará TPH automáticamente

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.ClienteId);

                entity.HasIndex(e => e.Identificacion)
                    .IsUnique()
                    .HasDatabaseName("IX_Clientes_Identificacion_Unique");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Genero)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Identificacion)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(200);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20);

                entity.Property(e => e.Contrasena)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasDefaultValue(true);
            });

            // Configuración de Cuenta
            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.NumeroCuenta)
                    .IsUnique()
                    .HasDatabaseName("IX_Cuentas_NumeroCuenta_Unique");

                entity.Property(e => e.NumeroCuenta)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TipoCuenta)
                    .IsRequired()
                    .HasConversion<string>(); // Guarda el enum como string

                entity.Property(e => e.SaldoInicial)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Relación con Cliente
                entity.HasOne(e => e.Cliente)
                    .WithMany(c => c.Cuentas)
                    .HasForeignKey(e => e.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict); // Evita borrado en cascada
            });

            // Configuración de Movimiento
            modelBuilder.Entity<Movimiento>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Fecha)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()"); // SQL Server

                entity.Property(e => e.TipoMovimiento)
                    .IsRequired()
                    .HasConversion<string>(); // Guarda el enum como string

                entity.Property(e => e.Valor)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Saldo)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                // Relación con Cuenta
                entity.HasOne(e => e.Cuenta)
                    .WithMany(c => c.Movimientos)
                    .HasForeignKey(e => e.CuentaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice para mejorar consultas por fecha
                entity.HasIndex(e => e.Fecha)
                    .HasDatabaseName("IX_Movimientos_Fecha");

                // Índice compuesto para reportes
                entity.HasIndex(e => new { e.CuentaId, e.Fecha })
                    .HasDatabaseName("IX_Movimientos_CuentaId_Fecha");
            });

            // Seed Data (Datos iniciales - opcional)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Datos de ejemplo de los casos de uso
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    ClienteId = 1,
                    Nombre = "Jose Lema",
                    Genero = "Masculino",
                    Edad = 35,
                    Identificacion = "1234567890",
                    Direccion = "Otavalo sn y principal",
                    Telefono = "0982547851",
                    Contrasena = "1234",
                    Estado = true
                },
                new Cliente
                {
                    ClienteId = 2,
                    Nombre = "Marianela Montalvo",
                    Genero = "Femenino",
                    Edad = 28,
                    Identificacion = "0987654321",
                    Direccion = "Amazonas y NNUU",
                    Telefono = "0975489655",
                    Contrasena = "5678",
                    Estado = true
                },
                new Cliente
                {
                    ClienteId = 3,
                    Nombre = "Juan Osorio",
                    Genero = "Masculino",
                    Edad = 42,
                    Identificacion = "1122334455",
                    Direccion = "13 junio y Equinoccial",
                    Telefono = "0988745871",
                    Contrasena = "1245",
                    Estado = true
                }
            );

            modelBuilder.Entity<Cuenta>().HasData(
                new Cuenta
                {
                    Id = 1,
                    NumeroCuenta = "478758",
                    TipoCuenta = TipoCuenta.Ahorro,
                    SaldoInicial = 2000,
                    Estado = true,
                    ClienteId = 1
                },
                new Cuenta
                {
                    Id = 2,
                    NumeroCuenta = "225487",
                    TipoCuenta = TipoCuenta.Corriente,
                    SaldoInicial = 100,
                    Estado = true,
                    ClienteId = 2
                },
                new Cuenta
                {
                    Id = 3,
                    NumeroCuenta = "495878",
                    TipoCuenta = TipoCuenta.Ahorro,
                    SaldoInicial = 0,
                    Estado = true,
                    ClienteId = 3
                },
                new Cuenta
                {
                    Id = 4,
                    NumeroCuenta = "496825",
                    TipoCuenta = TipoCuenta.Ahorro,
                    SaldoInicial = 540,
                    Estado = true,
                    ClienteId = 2
                }
            );
        }
    }