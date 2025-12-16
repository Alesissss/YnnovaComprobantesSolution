use ynnovaco_corpsaf_comprobantes;

-- DROPS TABLES
DROP TABLE IF EXISTS banco;
DROP TABLE IF EXISTS comprobante;
DROP TABLE IF EXISTS concepto;
DROP TABLE IF EXISTS empresa;
DROP TABLE IF EXISTS estado;
DROP TABLE IF EXISTS gasto;
DROP TABLE IF EXISTS moneda;
DROP TABLE IF EXISTS observaciones;
DROP TABLE IF EXISTS tipo_comprobante;
DROP TABLE IF EXISTS tipo_gasto;
DROP TABLE IF EXISTS tipo_rendicion;
DROP TABLE IF EXISTS tipo_usuario;
DROP TABLE IF EXISTS usuario;
DROP TABLE IF EXISTS empresa_usuario;

-- TABLA banco
CREATE TABLE banco (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    codigo VARCHAR(10) NOT NULL,
    descripcion VARCHAR(255) NOT NULL,
    estado BIT NOT NULL DEFAULT 1
);

-- TABLA comprobante
CREATE TABLE comprobante (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    anticipo_id INT,
    tipo_comprobante_id INT NOT NULL,
    concepto_id INT,
    concepto_otro VARCHAR(255),
    serie VARCHAR(4),
    numero VARCHAR(10),
    ruc_empresa VARCHAR(20),
    monto DECIMAL(10, 2) NOT NULL,
    fecha DATE NOT NULL,
    detalle TEXT,
    archivo VARCHAR(255),
    estado_id INT NOT NULL,
    moneda_id INT NOT NULL
);

-- TABLA concepto
CREATE TABLE concepto (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    estado BIT NOT NULL DEFAULT 1
);


-- TABLA empresa
CREATE TABLE empresa (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ruc char(11) NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    descripcion TEXT,
    estado BIT NOT NULL DEFAULT 1
);


-- TABLA estado
CREATE TABLE estado (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(30) NOT NULL,
    tabla VARCHAR(30) NOT NULL
);


-- TABLA gasto
CREATE TABLE gasto (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    fecha_hora DATETIME NOT NULL,
    importe DECIMAL(12, 2) NOT NULL,
    descripcion TEXT,
    empresa_id INT NOT NULL,
    banco_id INT NOT NULL,
    tipo_rendicion_id INT NOT NULL,
    usuario_id INT NOT NULL,
    tipo_gasto_id INT NOT NULL,
    estado_id INT NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
);

-- TABLA moneda
CREATE TABLE moneda (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(20) NOT NULL,
    simbolo VARCHAR(20) NOT NULL
);

-- TABLA observaciones
CREATE TABLE observaciones (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    comprobante_id INT NOT NULL,
    user_id INT NOT NULL,
    prioridad CHAR(1) NOT NULL, -- 'A' Alta, 'M' Media, 'B' Baja
    mensaje TEXT NOT NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE(),
    archivo VARCHAR(255)
);

-- TABLA tipo_comprobante
CREATE TABLE tipo_comprobante (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    codigo VARCHAR(255) NOT NULL,
    descripcion TEXT NOT NULL,
    estado BIT NOT NULL
);


-- TABLA tipo_gasto
CREATE TABLE tipo_gasto (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    estado BIT NOT NULL
);


-- TABLA tipo_rendicion
CREATE TABLE tipo_rendicion (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL,
    descripcion VARCHAR(255) NOT NULL,
    estado BIT NOT NULL DEFAULT 1
);


-- TABLA tipo_usuario
CREATE TABLE tipo_usuario (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(30) NOT NULL,
    estado BIT NOT NULL
);


-- TABLA usuario
CREATE TABLE usuario (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    dni CHAR(8) NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    telefono VARCHAR(20),
    password VARCHAR(255) NOT NULL,
    estado BIT NOT NULL
);

-- TABLA empresa_usuario
CREATE TABLE empresa_usuario (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    empresa_id INT NOT NULL,
    usuario_id INT NOT NULL,
    tipo_usuario_id INT NOT NULL
);