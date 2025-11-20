-- Script para crear las bases de datos del Proyecto NPS
-- Ejecútalo en SQL Server

-- --------------------------------------------
-- 1. BASE DE DATOS DE USUARIOS (Identity)
-- --------------------------------------------

-- Crear la base de datos si no existe
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'NPS_Identity_DB')
BEGIN
    CREATE DATABASE NPS_Identity_DB
END
GO

USE NPS_Identity_DB
GO

-- Tabla de Usuarios
-- Aquí guardamos el login, la contraseña encriptada y el rol (Admin o Votante)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Username NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        PasswordSalt NVARCHAR(MAX) NOT NULL,
        Role NVARCHAR(50) NOT NULL,          -- 'ADMIN' o 'VOTANTE'
        IsBlocked BIT NOT NULL DEFAULT 0,    -- Para bloquear si falla mucho
        FailedAttempts INT NOT NULL DEFAULT 0,
        RefreshToken NVARCHAR(MAX) NULL,
        RefreshTokenExpiryTime DATETIME2 NULL,
        CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        LastActivityUtc DATETIME2 NULL
    )
END
GO

-- --------------------------------------------
-- 2. BASE DE DATOS DE VOTOS (NPS)
-- --------------------------------------------

-- Crear la base de datos si no existe
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'NPS_Votes_DB')
BEGIN
    CREATE DATABASE NPS_Votes_DB
END
GO

USE NPS_Votes_DB
GO

-- Tabla de Votos
-- Aquí se guarda la calificación (0-10) y el comentario
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Votes' AND xtype='U')
BEGIN
    CREATE TABLE Votes (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,    -- ID del usuario que votó
        Score INT NOT NULL,                  -- Nota del 0 al 10
        Comment NVARCHAR(500) NULL,          -- Opinión opcional
        CreatedAtUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        -- Validamos que la nota sea válida (0-10)
        CONSTRAINT CK_Score_Range CHECK (Score >= 0 AND Score <= 10)
    )
END
GO

-- ¡Listo! Las tablas están creadas.
-- Nota: El usuario 'admin' se crea solo cuando arrancas la aplicación.