-- 02 - Tablas, claves primarias, foráneas y restricciones
USE LogiTrackDB;
GO

CREATE TABLE Roles (
    Id     INT IDENTITY PRIMARY KEY,
    Nombre VARCHAR(30) NOT NULL CONSTRAINT UQ_Roles_Nombre UNIQUE
);

CREATE TABLE Usuarios (
    Id        INT IDENTITY PRIMARY KEY,
    Nombre    VARCHAR(100) NOT NULL,
    Email     VARCHAR(150) NOT NULL CONSTRAINT UQ_Usuarios_Email UNIQUE,
    RolId     INT NOT NULL,
    FechaAlta DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolId) REFERENCES Roles(Id)
);

-- Auto-referenciada: sostiene el CTE recursivo de categorías.
CREATE TABLE Categorias (
    Id               INT IDENTITY PRIMARY KEY,
    Nombre           VARCHAR(80) NOT NULL,
    CategoriaPadreId INT NULL,
    CONSTRAINT FK_Categorias_Padre FOREIGN KEY (CategoriaPadreId) REFERENCES Categorias(Id)
);

-- Tabla base: lo común de los 3 tipos de producto.
CREATE TABLE Productos (
    Id          INT IDENTITY PRIMARY KEY,
    SKU         VARCHAR(20) NOT NULL CONSTRAINT UQ_Productos_SKU UNIQUE,
    Nombre      VARCHAR(120) NOT NULL,
    PrecioBase  DECIMAL(12,2) NOT NULL CONSTRAINT CK_Productos_Precio CHECK (PrecioBase > 0),
    Tipo        VARCHAR(10) NOT NULL CONSTRAINT CK_Productos_Tipo CHECK (Tipo IN ('Fisico','Digital','Servicio')),
    CategoriaId INT NULL,
    Activo      BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
);

-- Tablas hijas 1-a-1: cada tipo guarda solo lo que le es propio.
CREATE TABLE ProductoFisicos (
    ProductoId INT PRIMARY KEY,
    PesoKg     DECIMAL(8,2) NOT NULL CONSTRAINT CK_Fisicos_Peso CHECK (PesoKg > 0),
    CONSTRAINT FK_Fisicos_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);

CREATE TABLE ProductoDigitales (
    ProductoId  INT PRIMARY KEY,
    UrlDescarga VARCHAR(300) NOT NULL,
    CONSTRAINT FK_Digitales_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);

CREATE TABLE Servicios (
    ProductoId    INT PRIMARY KEY,
    FechaAgenda   DATETIME NOT NULL,
    DuracionHoras DECIMAL(4,1) NOT NULL CONSTRAINT CK_Servicios_Duracion CHECK (DuracionHoras > 0),
    CONSTRAINT FK_Servicios_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);

-- Solo productos físicos tienen inventario.
CREATE TABLE Inventario (
    ProductoId   INT PRIMARY KEY,
    StockInicial INT NOT NULL CONSTRAINT CK_Inventario_Inicial CHECK (StockInicial >= 0),
    StockActual  INT NOT NULL CONSTRAINT CK_Inventario_Actual CHECK (StockActual >= 0),
    CONSTRAINT FK_Inventario_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);

CREATE TABLE Pedidos (
    Id        INT IDENTITY PRIMARY KEY,
    ClienteId INT NOT NULL,
    Fecha     DATETIME NOT NULL DEFAULT GETDATE(),
    Estado    VARCHAR(15) NOT NULL DEFAULT 'Pendiente'
              CONSTRAINT CK_Pedidos_Estado CHECK (Estado IN ('Pendiente','Confirmado','EnPreparacion','Enviado','Entregado','Cancelado')),
    Total     DECIMAL(12,2) NOT NULL DEFAULT 0 CONSTRAINT CK_Pedidos_Total CHECK (Total >= 0),
    CONSTRAINT FK_Pedidos_Usuarios FOREIGN KEY (ClienteId) REFERENCES Usuarios(Id)
);

CREATE TABLE PedidoDetalles (
    Id             INT IDENTITY PRIMARY KEY,
    PedidoId       INT NOT NULL,
    ProductoId     INT NOT NULL,
    Cantidad       INT NOT NULL CONSTRAINT CK_Detalles_Cantidad CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(12,2) NOT NULL CONSTRAINT CK_Detalles_Precio CHECK (PrecioUnitario > 0),
    CONSTRAINT FK_Detalles_Pedidos FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id),
    CONSTRAINT FK_Detalles_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);

CREATE TABLE Pagos (
    Id          INT IDENTITY PRIMARY KEY,
    PedidoId    INT NOT NULL,
    Metodo      VARCHAR(20) NOT NULL CONSTRAINT CK_Pagos_Metodo CHECK (Metodo IN ('Tarjeta','Transferencia')),
    Monto       DECIMAL(12,2) NOT NULL CONSTRAINT CK_Pagos_Monto CHECK (Monto > 0),
    Comprobante VARCHAR(30) NOT NULL CONSTRAINT UQ_Pagos_Comprobante UNIQUE,
    Fecha       DATETIME NOT NULL DEFAULT GETDATE(),
    Estado      VARCHAR(10) NOT NULL CONSTRAINT CK_Pagos_Estado CHECK (Estado IN ('Aprobado','Rechazado')),
    CONSTRAINT FK_Pagos_Pedidos FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id)
);

CREATE TABLE AuditoriaInventario (
    Id            INT IDENTITY PRIMARY KEY,
    ProductoId    INT NOT NULL,
    Operacion     VARCHAR(15) NOT NULL CONSTRAINT CK_Auditoria_Operacion CHECK (Operacion IN ('Venta','Reposicion','Ajuste')),
    StockAnterior INT NOT NULL,
    StockNuevo    INT NOT NULL,
    Fecha         DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Auditoria_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);
GO
