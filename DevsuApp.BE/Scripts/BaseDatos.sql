IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Clientes] (
    [ClienteId] int NOT NULL IDENTITY,
    [Contrasena] nvarchar(100) NOT NULL,
    [Estado] bit NOT NULL DEFAULT CAST(1 AS bit),
    [PersonaId] int NOT NULL,
    [Nombre] nvarchar(100) NOT NULL,
    [Genero] nvarchar(10) NOT NULL,
    [Edad] int NOT NULL,
    [Identificacion] nvarchar(20) NOT NULL,
    [Direccion] nvarchar(200) NULL,
    [Telefono] nvarchar(20) NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([ClienteId])
);

CREATE TABLE [Cuentas] (
    [Id] int NOT NULL IDENTITY,
    [NumeroCuenta] nvarchar(20) NOT NULL,
    [TipoCuenta] nvarchar(max) NOT NULL,
    [SaldoInicial] decimal(18,2) NOT NULL,
    [Estado] bit NOT NULL DEFAULT CAST(1 AS bit),
    [ClienteId] int NOT NULL,
    CONSTRAINT [PK_Cuentas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cuentas_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([ClienteId]) ON DELETE NO ACTION
);

CREATE TABLE [Movimientos] (
    [Id] int NOT NULL IDENTITY,
    [Fecha] datetime2 NOT NULL DEFAULT (GETDATE()),
    [TipoMovimiento] nvarchar(max) NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    [Saldo] decimal(18,2) NOT NULL,
    [CuentaId] int NOT NULL,
    CONSTRAINT [PK_Movimientos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Movimientos_Cuentas_CuentaId] FOREIGN KEY ([CuentaId]) REFERENCES [Cuentas] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ClienteId', N'Contrasena', N'Direccion', N'Edad', N'Estado', N'Genero', N'Identificacion', N'Nombre', N'PersonaId', N'Telefono') AND [object_id] = OBJECT_ID(N'[Clientes]'))
    SET IDENTITY_INSERT [Clientes] ON;
INSERT INTO [Clientes] ([ClienteId], [Contrasena], [Direccion], [Edad], [Estado], [Genero], [Identificacion], [Nombre], [PersonaId], [Telefono])
VALUES (1, N'1234', N'Otavalo sn y principal', 35, CAST(1 AS bit), N'Masculino', N'1234567890', N'Jose Lema', 0, N'0982547851'),
(2, N'5678', N'Amazonas y NNUU', 28, CAST(1 AS bit), N'Femenino', N'0987654321', N'Marianela Montalvo', 0, N'0975489655'),
(3, N'1245', N'13 junio y Equinoccial', 42, CAST(1 AS bit), N'Masculino', N'1122334455', N'Juan Osorio', 0, N'0988745871');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ClienteId', N'Contrasena', N'Direccion', N'Edad', N'Estado', N'Genero', N'Identificacion', N'Nombre', N'PersonaId', N'Telefono') AND [object_id] = OBJECT_ID(N'[Clientes]'))
    SET IDENTITY_INSERT [Clientes] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClienteId', N'Estado', N'NumeroCuenta', N'SaldoInicial', N'TipoCuenta') AND [object_id] = OBJECT_ID(N'[Cuentas]'))
    SET IDENTITY_INSERT [Cuentas] ON;
INSERT INTO [Cuentas] ([Id], [ClienteId], [Estado], [NumeroCuenta], [SaldoInicial], [TipoCuenta])
VALUES (1, 1, CAST(1 AS bit), N'478758', 2000.0, N'Ahorro'),
(2, 2, CAST(1 AS bit), N'225487', 100.0, N'Corriente'),
(3, 3, CAST(1 AS bit), N'495878', 0.0, N'Ahorro'),
(4, 2, CAST(1 AS bit), N'496825', 540.0, N'Ahorro');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClienteId', N'Estado', N'NumeroCuenta', N'SaldoInicial', N'TipoCuenta') AND [object_id] = OBJECT_ID(N'[Cuentas]'))
    SET IDENTITY_INSERT [Cuentas] OFF;

CREATE UNIQUE INDEX [IX_Clientes_Identificacion_Unique] ON [Clientes] ([Identificacion]);

CREATE INDEX [IX_Cuentas_ClienteId] ON [Cuentas] ([ClienteId]);

CREATE UNIQUE INDEX [IX_Cuentas_NumeroCuenta_Unique] ON [Cuentas] ([NumeroCuenta]);

CREATE INDEX [IX_Movimientos_CuentaId_Fecha] ON [Movimientos] ([CuentaId], [Fecha]);

CREATE INDEX [IX_Movimientos_Fecha] ON [Movimientos] ([Fecha]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251027012050_InitialEntities', N'9.0.10');

COMMIT;
GO

