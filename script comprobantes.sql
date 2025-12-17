-- USE DB
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

-- CREATE TABLE
-- TABLA banco
CREATE TABLE banco (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    codigo VARCHAR(50) NOT NULL,
    descripcion VARCHAR(255) NOT NULL,
    estado BIT NOT NULL
);

-- TABLA comprobante
CREATE TABLE comprobante (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    usuario_id INT NOT NULL,
    gasto_id INT,
    tipo_comprobante_id INT NOT NULL,
    concepto_id INT,
    concepto_otro VARCHAR(255),
    serie VARCHAR(4),
    numero VARCHAR(10),
    ruc_empresa CHAR(11),
    monto DECIMAL(10, 2) NOT NULL,
    fecha DATE NOT NULL,
    descripcion TEXT,
    archivo TEXT,
    estado_id INT NOT NULL,
    moneda_id INT NOT NULL,
    fecha_registro DATETIME DEFAULT GETDATE()
);

-- TABLA concepto
CREATE TABLE concepto (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    estado BIT NOT NULL
);


-- TABLA empresa
CREATE TABLE empresa (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ruc char(11) NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    descripcion TEXT,
    estado BIT NOT NULL
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
    fecha DATE NOT NULL,
    importe DECIMAL(12, 2) NULL,
    descripcion TEXT NULL,
    empresa_id INT NOT NULL,
    banco_id INT NULL,
    tipo_rendicion_id INT NULL,
    usuario_id INT NOT NULL,
    tipo_gasto_id INT NOT NULL,
    moneda_id INT NULL,
    estado_id INT NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
);

-- TABLA moneda
CREATE TABLE moneda (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    simbolo VARCHAR(50) NOT NULL
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
    codigo VARCHAR(50) NOT NULL,
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
    codigo VARCHAR(50) NOT NULL,
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

-- INSERTS
-- INSERTS DE 'tipo_gasto'
INSERT INTO tipo_gasto (nombre, estado) VALUES ('ANTICIPO', 1);
INSERT INTO tipo_gasto (nombre, estado) VALUES ('REEMBOLSO', 1);

-- INSERTS DE 'tipo_rendicion'
INSERT INTO tipo_rendicion (codigo, descripcion, estado) VALUES ('1', 'VIÁTICOS', 1);
INSERT INTO tipo_rendicion (codigo, descripcion, estado) VALUES ('2', 'COMISIÓN DE SERVICIOS', 1);
INSERT INTO tipo_rendicion (codigo, descripcion, estado) VALUES ('3', 'GASTOS DE REPRESENTACIÓN', 1);
INSERT INTO tipo_rendicion (codigo, descripcion, estado) VALUES ('3', 'GASTOS OPERACIÓN  ', 1);

-- INSERTS DE 'estado' PARA LA TABLA 'gasto'
INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'GASTO');
INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'GASTO');
INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'GASTO');
INSERT INTO estado (nombre, tabla) VALUES ('En observación', 'GASTO');

-- INSERTS DE 'estado' PARA LA TABLA 'comprobante'
INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'COMPROBANTE');
INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'COMPROBANTE');
INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'COMPROBANTE');
INSERT INTO estado (nombre, tabla) VALUES ('En observación', 'COMPROBANTE');

-- INSERTS DE 'moneda'
INSERT INTO moneda (nombre, simbolo) VALUES ('Soles', 'S/.');
INSERT INTO moneda (nombre, simbolo) VALUES ('Dólares', '$');

-- INSERTS DE 'banco'
INSERT INTO banco (codigo, descripcion, estado) VALUES ('1','CENTRAL RESERVA DEL PERU',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('2','DE CREDITO DEL PERU',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('3','INTERNACIONAL DEL PERU',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('5','LATINO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('7','CITIBANK DEL PERU S.A.',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('8','STANDARD CHARTERED',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('9','SCOTIABANK PERU',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('11','CONTINENTAL',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('12','DE LIMA',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('16','MERCANTIL',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('18','NACION',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('22','SANTANDER CENTRAL HISPANO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('23','DE COMERCIO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('25','REPUBLICA',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('26','NBK BANK',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('29','BANCOSUR',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('35','FINANCIERO DEL PERU',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('37','DEL PROGRESO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('38','INTERAMERICANO FINANZAS',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('39','BANEX',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('40','NUEVO MUNDO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('41','SUDAMERICANO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('42','DEL LIBERTADOR',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('43','DEL TRABAJO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('44','SOLVENTA',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('45','SERBANCO SA.',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('46','BANK OF BOSTON',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('47','ORION',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('48','DEL PAIS',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('49','MI BANCO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('50','BNP PARIBAS',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('51','AGROBANCO',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('53','HSBC BANK PERU S.A.',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('54','BANCO FALABELLA S.A.',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('55','BANCO RIPLEY',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('56','BANCO SANTANDER PERU S.A.',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('58','BANCO AZTECA DEL PERU',1);
INSERT INTO banco (codigo, descripcion, estado) VALUES ('99','OTROS',1);