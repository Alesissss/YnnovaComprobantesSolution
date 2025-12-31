-- =============================================
-- SCRIPT DE BASE DE DATOS - LIQUIDACION (AGIL / SIN FKs)
-- =============================================

USE ynnovaco_corpsaf_comprobantes;
GO

-- 1. DROP TABLES (Eliminamos todo para reiniciar limpio)
DROP TABLE IF EXISTS detalle_planilla_movilidad;
DROP TABLE IF EXISTS planilla_movilidad;
DROP TABLE IF EXISTS comprobante;
DROP TABLE IF EXISTS observacion;
DROP TABLE IF EXISTS reembolso;
DROP TABLE IF EXISTS anticipo;
DROP TABLE IF EXISTS liquidacion;
DROP TABLE IF EXISTS empresa_usuario;
DROP TABLE IF EXISTS usuario;
DROP TABLE IF EXISTS empresa;
DROP TABLE IF EXISTS banco;
DROP TABLE IF EXISTS concepto;
DROP TABLE IF EXISTS tipo_comprobante;
DROP TABLE IF EXISTS tipo_rendicion;
DROP TABLE IF EXISTS tipo_usuario;
DROP TABLE IF EXISTS estado;
DROP TABLE IF EXISTS moneda;
GO

-- 2. TABLAS MAESTRAS (Configuración)
CREATE TABLE estado (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    tabla VARCHAR(50) NOT NULL
);

CREATE TABLE moneda (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    simbolo VARCHAR(10) NOT NULL
);

CREATE TABLE banco (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    codigo VARCHAR(50) NOT NULL,
    descripcion VARCHAR(255) NOT NULL,
    estado BIT NOT NULL DEFAULT 1
);

CREATE TABLE empresa (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ruc CHAR(11) NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    descripcion TEXT,
    estado BIT NOT NULL DEFAULT 1
);

CREATE TABLE tipo_usuario (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(30) NOT NULL,
    estado BIT NOT NULL DEFAULT 1
);

CREATE TABLE usuario (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    dni CHAR(8) NOT NULL,
    nombre VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    telefono VARCHAR(20),
    numero_cuenta VARCHAR(20),
    password VARCHAR(255) NOT NULL,
    estado BIT NOT NULL DEFAULT 1
);

CREATE TABLE empresa_usuario (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    empresa_id INT NOT NULL,
    usuario_id INT NOT NULL,
    tipo_usuario_id INT NOT NULL
);

CREATE TABLE concepto (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    estado BIT NOT NULL DEFAULT 1
);

CREATE TABLE tipo_comprobante (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    codigo VARCHAR(50) NOT NULL,
    descripcion TEXT NOT NULL,
    estado BIT NOT NULL DEFAULT 1
);

CREATE TABLE tipo_rendicion (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    codigo VARCHAR(50) NOT NULL,
    descripcion VARCHAR(255) NOT NULL,
    estado BIT NOT NULL DEFAULT 1
);

-- 3. TABLA PADRE: LIQUIDACIÓN (La Carpeta)
CREATE TABLE liquidacion (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    codigo_generado VARCHAR(20), -- Ej: LIQ-0001-2025
    empresa_id INT NOT NULL,
    usuario_id INT NOT NULL,
    
    fecha_inicio DATE NOT NULL DEFAULT GETDATE(),
    fecha_cierre DATE NULL,
    descripcion VARCHAR(MAX),
    
    -- RESUMEN FINANCIERO (Calculados en C# y guardados aquí para reporte rápido)
    total_anticibido DECIMAL(12,2) DEFAULT 0, -- Suma de anticipos TRANSFERIDOS
    total_gastado DECIMAL(12,2) DEFAULT 0,    -- Suma de comprobantes + planillas APROBADAS
    saldo_final DECIMAL(12,2) DEFAULT 0,      -- Diferencia (Positivo=Devolver, Negativo=Reembolsar)
    
    estado_id INT NOT NULL, -- (Abierta, Cerrada)
    usuario_registro INT,
    fecha_registro DATETIME DEFAULT GETDATE()
);

-- 4. TABLA ANTICIPO (Dinero que entra)
CREATE TABLE anticipo (
    id INT IDENTITY(1,1) PRIMARY KEY,
    liquidacion_id INT NOT NULL, -- Relación lógica con Liquidación
    
    banco_id INT,
    moneda_id INT,
    tipo_rendicion_id INT,
    monto DECIMAL(12,2) NOT NULL,
    descripcion VARCHAR(MAX),
    fecha_solicitud DATE,
    fecha_limite_rendicion DATE,
    
    -- === CAMPOS DEL VOUCHER / TRANSFERENCIA (SOLICITADO) ===
    voucher_numero_operacion VARCHAR(50) NULL, -- Código de operación bancaria
    voucher_fecha DATETIME NULL,               -- Cuándo hizo la transferencia el admin
    voucher_archivo_url VARCHAR(MAX) NULL,     -- La foto/pdf del voucher
    voucher_banco_origen_id INT NULL,          -- Desde qué banco pagó la empresa (opcional)
    -- ========================================================
    
    estado_id INT, -- (Generado, Transferido)
    usuario_aprobador INT, -- El admin que subió el voucher
    fecha_registro DATETIME DEFAULT GETDATE()
);

-- 5. TABLA REEMBOLSO / DEVOLUCION (MODIFICADA PARA SOPORTAR VOUCHERS)
CREATE TABLE reembolso (
    id INT IDENTITY(1,1) PRIMARY KEY,
    liquidacion_id INT NOT NULL,
    
    fecha_solicitud DATE,
    moneda_id INT,
    monto DECIMAL(12,2), 
    descripcion VARCHAR(MAX),
    
    banco_id INT,
    numero_cuenta VARCHAR(50),
    
    es_devolucion BIT DEFAULT 0, -- 1: Usuario devuelve a Empresa. 0: Empresa reembolsa a Usuario.
    
    -- === NUEVOS CAMPOS DE VOUCHER (Igual que Anticipo) ===
    voucher_numero_operacion VARCHAR(50) NULL,
    voucher_fecha DATETIME NULL,
    voucher_archivo_url VARCHAR(MAX) NULL,
    -- =====================================================

    estado_id INT,
    usuario_registro INT, -- Quien subió el registro (Admin o Usuario)
    fecha_registro DATETIME DEFAULT GETDATE()
);

-- 6. TABLA PLANILLA MOVILIDAD (Dinero que sale sin factura)
CREATE TABLE planilla_movilidad (
    id INT IDENTITY(1,1) PRIMARY KEY,
    liquidacion_id INT NOT NULL, 
    numero_planilla VARCHAR(255),
    fecha_emision DATE,
    
    -- El monto total se suma de sus detalles
    monto_total_declarado DECIMAL(10,2) DEFAULT 0, 
    
    estado_id INT,
    fecha_registro DATETIME DEFAULT GETDATE()
);

CREATE TABLE detalle_planilla_movilidad (
    id INT IDENTITY(1,1) PRIMARY KEY,
    planilla_movilidad_id INT NOT NULL,
    
    fecha_gasto DATE,
    motivo VARCHAR(255),
    lugar_origen VARCHAR(255),
    lugar_destino VARCHAR(255),
    monto DECIMAL(10,2),
    
    estado_aprobacion BIT DEFAULT 1 -- 1: Aprobado, 0: Rechazado (Para cálculo fino)
);

-- 7. TABLA COMPROBANTE (Dinero que sale con factura)
CREATE TABLE comprobante (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    liquidacion_id INT NOT NULL, -- Pertenece a la carpeta
    
    -- Opcional: Referencia lógica si el usuario quiere decir "esto es de tal anticipo"
    anticipo_id INT NULL, 
    
    tipo_comprobante_id INT NOT NULL,
    concepto_id INT,
    proveedor_nombre VARCHAR(255),
    ruc_empresa CHAR(11),
    serie VARCHAR(20),
    numero VARCHAR(20),
    fecha_emision DATE NOT NULL,
    moneda_id INT NOT NULL,
    monto_total DECIMAL(10, 2) NOT NULL,
    descripcion VARCHAR(MAX),
    archivo_url VARCHAR(MAX),
    
    estado_id INT NOT NULL, -- Pendiente, Aprobado, Rechazado
    usuario_aprobador INT,
    usuario_registro INT,
    fecha_registro DATETIME DEFAULT GETDATE()
);

-- 8. OBSERVACIONES (Chat de la Liquidación)
CREATE TABLE observacion (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    liquidacion_id INT NOT NULL,
    usuario_id INT NOT NULL,
    prioridad CHAR(1) NOT NULL, -- 'A', 'M', 'B'
    mensaje TEXT NOT NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
);

-- =============================================
-- 9. INSERTS DE DATOS INICIALES (PARA QUE CORRA YA)
-- =============================================

-- Estados para LIQUIDACIÓN
INSERT INTO estado (nombre, tabla) VALUES ('Abierta', 'LIQUIDACION');
INSERT INTO estado (nombre, tabla) VALUES ('Cerrada', 'LIQUIDACION');

-- Estados para ANTICIPO (Flujo del Admin)
INSERT INTO estado (nombre, tabla) VALUES ('Generado', 'ANTICIPO');  -- Cuando el Admin registra el monto
INSERT INTO estado (nombre, tabla) VALUES ('Transferido', 'ANTICIPO'); -- Cuando el Admin sube el voucher (Dinero en manos del usuario)

-- Estados para COMPROBANTE (Flujo de aprobación)
INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'COMPROBANTE'); -- Usuario sube foto
INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'COMPROBANTE');  -- Admin da OK
INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'COMPROBANTE'); -- Admin rechaza
INSERT INTO estado (nombre, tabla) VALUES ('Observado', 'COMPROBANTE'); -- Opcional: si quieres un estado intermedio por chat

-- Estados para REEMBOLSO (Cierre final)
INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'REEMBOLSO');
INSERT INTO estado (nombre, tabla) VALUES ('Pagado', 'REEMBOLSO');

-- Estados para PLANILLA (Cabecera)
INSERT INTO estado (nombre, tabla) VALUES ('Activo', 'PLANILLA_MOVILIDAD');

-- INSERTS
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

-- INSERTS DE 'estado' PARA LA TABLA 'comprobante'
--INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'COMPROBANTE');
--INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'COMPROBANTE');
--INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'COMPROBANTE');
--INSERT INTO estado (nombre, tabla) VALUES ('Observado', 'COMPROBANTE');
--INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'ANTICIPO');
--INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'ANTICIPO');
--INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'ANTICIPO');
--INSERT INTO estado (nombre, tabla) VALUES ('Observado', 'ANTICIPO');
--INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'REEMBOLSO');
--INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'REEMBOLSO');
--INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'REEMBOLSO');
--INSERT INTO estado (nombre, tabla) VALUES ('Observado', 'REEMBOLSO');
--INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'PLANILLA_MOVILIDAD');
--INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'PLANILLA_MOVILIDAD');
--INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'PLANILLA_MOVILIDAD');
--INSERT INTO estado (nombre, tabla) VALUES ('Observado', 'PLANILLA_MOVILIDAD');
--INSERT INTO estado (nombre, tabla) VALUES ('Pendiente', 'DEVOLUCION_ANTICIPO');
--INSERT INTO estado (nombre, tabla) VALUES ('Aprobado', 'DEVOLUCION_ANTICIPO');
--INSERT INTO estado (nombre, tabla) VALUES ('Rechazado', 'DEVOLUCION_ANTICIPO');
--INSERT INTO estado (nombre, tabla) VALUES ('Observado', 'DEVOLUCION_ANTICIPO');

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
