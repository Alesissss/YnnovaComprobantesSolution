-- USE DB
use ynnovaco_corpsaf_comprobantes;

-- DROPS TABLES
DROP TABLE IF EXISTS devolucion_gasto;
DROP TABLE IF EXISTS banco;
DROP TABLE IF EXISTS comprobante;
DROP TABLE IF EXISTS concepto;
DROP TABLE IF EXISTS empresa;
DROP TABLE IF EXISTS estado;
DROP TABLE IF EXISTS gasto;
DROP TABLE IF EXISTS moneda;
DROP TABLE IF EXISTS observacion;
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
    usuario_aprobador INT NULL,
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
    usuario_aprobador INT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
);

-- TABLA devolucion_gasto
CREATE TABLE devolucion_gasto (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    fecha DATE NOT NULL,
    importe DECIMAL(12, 2) NULL,
    descripcion TEXT NULL,
    empresa_id INT NOT NULL,
    banco_id INT NULL,
    estado_id INT NOT NULL,
    usuario_aprobador INT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE()
);

-- TABLA moneda
CREATE TABLE moneda (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    simbolo VARCHAR(50) NOT NULL
);

-- TABLA observacion
CREATE TABLE observacion (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    comprobante_id INT NOT NULL,
    usuario_id INT NOT NULL,
    prioridad CHAR(1) NOT NULL, -- 'A' Alta, 'M' Media, 'B' Baja
    mensaje TEXT NOT NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
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

-- INSERTS DE 'tipo_comprobante'
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('00','Otros',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('01','Factura',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('02','Recibo por Honorarios',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('03','Boleta de Venta',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('04','Liquidación de compra',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('05','Boleto de compañía de aviación comercial por el servicio de transporte aéreo de pasajeros',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('06','Carta de porte aéreo por el servicio de transporte de carga aérea',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('07','Nota de crédito',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('08','Nota de débito',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('09','Guía de remisión - Remitente',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('10','Recibo por Arrendamiento',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('11','Póliza emitida por las Bolsas de Valores, Bolsas de Productos o Agentes de Intermediación por operaciones realizadas en las Bolsas de Valores o Productos o fuera de las mismas, autorizadas por CONASEV',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('12','Ticket o cinta emitido por máquina registradora',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('13','Documento emitido por bancos, instituciones financieras, crediticias y de seguros que se encuentren bajo el control de la Superintendencia de Banca y Seguros',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('14','Recibo por servicios públicos de suministro de energía eléctrica, agua, teléfono, telex y telegráficos y otros servicios complementarios que se incluyan en el recibo de servicio público',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('15','Boleto emitido por las empresas de transporte público urbano de pasajeros',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('16','Boleto de viaje emitido por las empresas de transporte público interprovincial de pasajeros dentro del país',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('17','Documento emitido por la Iglesia Católica por el arrendamiento de bienes inmuebles',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('18','Documento emitido por las Administradoras Privadas de Fondo de Pensiones que se encuentran bajo la supervisión de la Superintendencia de Administradoras Privadas de Fondos de Pensiones',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('19','Boleto o entrada por atracciones y espectáculos públicos',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('20','Comprobante de Retención',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('21','Conocimiento de embarque por el servicio de transporte de carga marítima',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('22','Comprobante por Operaciones No Habituales',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('23','Pólizas de Adjudicación emitidas con ocasión del remate o adjudicación de bienes por venta forzada, por los martilleros o las entidades que rematen o subasten bienes por cuenta de terceros',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('24','Certificado de pago de regalías emitidas por PERUPETRO S.A',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('25','Documento de Atribución (Ley del Impuesto General a las Ventas e Impuesto Selectivo al Consumo, Art. 19º, último párrafo, R.S. N° 022-98-SUNAT).',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('26','Recibo por el Pago de la Tarifa por Uso de Agua Superficial con fines agrarios y por el pago de la Cuota para la ejecución de una determinada obra o actividad acordada por la Asamblea General de la Comisión de Regantes o Resolución expedida por el Jefe de la Unidad de Aguas y de Riego (Decreto Supremo N° 003-90-AG, Arts. 28 y 48)',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('27','Seguro Complementario de Trabajo de Riesgo',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('28','Tarifa Unificada de Uso de Aeropuerto',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('29','Documentos emitidos por la COFOPRI en calidad de oferta de venta de terrenos, los correspondientes a las subastas públicas y a la retribución de los servicios que presta',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('30','Documentos emitidos por las empresas que desempeñan el rol adquirente en los sistemas de pago mediante tarjetas de crédito y débito',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('31','Guía de Remisión - Transportista',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('32','Documentos emitidos por las empresas recaudadoras de la denominada Garantía de Red Principal a la que hace referencia el numeral 7.6 del artículo 7° de la Ley N° 27133 – Ley de Promoción del Desarrollo de la Industria del Gas Natural',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('34','Documento del Operador',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('35','Documento del Partícipe',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('36','Recibo de Distribución de Gas Natural',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('37','Documentos que emitan los concesionarios del servicio de revisiones técnicas vehiculares, por la prestación de dicho servicio',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('40','Constancia de Depósito - IVAP (Ley 28211)',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('50','Declaración Única de Aduanas - Importación definitiva',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('52','Despacho Simplificado - Importación Simplificada',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('53','Declaración de Mensajería o Courier',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('54','Liquidación de Cobranza',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('87','Nota de Crédito Especial',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('88','Nota de Débito Especial',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('91','Comprobante de No Domiciliado',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('96','Exceso de crédito fiscal por retiro de bienes',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('97','Nota de Crédito - No Domiciliado',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('98','Nota de Débito - No Domiciliado',1);
INSERT INTO tipo_comprobante (codigo, descripcion, estado) VALUES ('99','Otros - Consolidado de Boletas de Venta',1);

-- INSERTS DE 'estado' PARA LA TABLA 'gasto'
INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'GASTO');
INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'GASTO');
INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'GASTO');
INSERT INTO estado (nombre, tabla) VALUES ('Observado', 'GASTO');

-- INSERTS DE 'estado' PARA LA TABLA 'comprobante'
INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'COMPROBANTE');
INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'COMPROBANTE');
INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'COMPROBANTE');
INSERT INTO estado (nombre, tabla) VALUES ('Observado', 'COMPROBANTE');

-- INSERTS DE 'moneda'
INSERT INTO moneda (nombre, simbolo) VALUES ('Soles', 'S/.');
INSERT INTO moneda (nombre, simbolo) VALUES ('Dólares', '$');

-- INSERTS DE 'concepto'
INSERT INTO concepto (nombre, descripcion, estado) VALUES ('ALIMENTACIÓN', 'Descripción de alimentación', 1);
INSERT INTO concepto (nombre, descripcion, estado) VALUES ('HOSPEDAJE', 'Descripción de hospedaje', 1);
INSERT INTO concepto (nombre, descripcion, estado) VALUES ('COMBUSTIBLE', 'Descripción de combustible', 1);

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

-- INSERTS DE 'tipo_usuario'
INSERT INTO tipo_usuario (nombre, estado) VALUES ('Administrador del sistema', 1);
INSERT INTO tipo_usuario (nombre, estado) VALUES ('Usuario', 1);

-- INSERTS DE 'empresa'
INSERT INTO empresa (ruc, nombre, descripcion, estado) VALUES ('12345678901', 'YNNOVA', 'SOMOS BUENOS', 1); 

-- INSERTS DE 'usuario'
INSERT INTO usuario (dni, nombre, email, telefono, password, estado) VALUES ('71870353', 'Sandro Bustamante Vasquez', 'furia241@gmail.com', '973702664', 'password123', 1);
INSERT INTO usuario (dni, nombre, email, telefono, password, estado) VALUES ('75090896', 'Alexis Torres Cabrejos', 'gfake040305@gmail.com', '999796517', 'password123', 1);
INSERT INTO usuario (dni, nombre, email, telefono, password, estado) VALUES ('77013712', 'Fernando José Dávila Ubillus', 'gerencia.tics@ynnovacorp.com', '963140425', 'password123', 1);

-- INSERTS DE 'empresa_usuario'
INSERT INTO empresa_usuario (empresa_id, usuario_id, tipo_usuario_id) VALUES (1, 1, 1);
INSERT INTO empresa_usuario (empresa_id, usuario_id, tipo_usuario_id) VALUES (1, 2, 1);
INSERT INTO empresa_usuario (empresa_id, usuario_id, tipo_usuario_id) VALUES (1, 3, 2);
