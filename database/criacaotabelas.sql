-- ============================================================
--  Sistema de Gerenciamento de Eventos
--  PUC Minas - Análise e Desenvolvimento de Sistemas
--  Script de Criação das Tabelas (DDL)
-- ============================================================

-- ------------------------------------------------------------
-- 1. CATEGORIA
-- ------------------------------------------------------------
CREATE TABLE Categoria (
    idCategoria     INT             NOT NULL AUTO_INCREMENT,
    nomeCategoria   VARCHAR(100)    NOT NULL,
    descricao       TEXT,
    CONSTRAINT pk_categoria PRIMARY KEY (idCategoria)
);

-- ------------------------------------------------------------
-- 2. LOCAL
-- ------------------------------------------------------------
CREATE TABLE Local (
    idLocal         INT             NOT NULL AUTO_INCREMENT,
    nomeLocal       VARCHAR(150)    NOT NULL,
    endereco        VARCHAR(255)    NOT NULL,
    cidade          VARCHAR(100)    NOT NULL,
    capacidade      INT             NOT NULL CHECK (capacidade > 0),
    CONSTRAINT pk_local PRIMARY KEY (idLocal)
);

-- ------------------------------------------------------------
-- 3. ORGANIZADOR
-- ------------------------------------------------------------
CREATE TABLE Organizador (
    idOrganizador   INT             NOT NULL AUTO_INCREMENT,
    nome            VARCHAR(150)    NOT NULL,
    email           VARCHAR(150)    NOT NULL UNIQUE,
    telefone        VARCHAR(20),
    CONSTRAINT pk_organizador PRIMARY KEY (idOrganizador)
);

-- ------------------------------------------------------------
-- 4. PARTICIPANTE
-- ------------------------------------------------------------
CREATE TABLE Participante (
    idParticipante  INT             NOT NULL AUTO_INCREMENT,
    nome            VARCHAR(150)    NOT NULL,
    cpf             CHAR(11)        NOT NULL UNIQUE,
    email           VARCHAR(150)    NOT NULL UNIQUE,
    telefone        VARCHAR(20),
    dataNascimento  DATE,
    CONSTRAINT pk_participante PRIMARY KEY (idParticipante)
);

-- ------------------------------------------------------------
-- 5. EVENTO
-- Depende de: Organizador, Categoria, Local
-- ------------------------------------------------------------
CREATE TABLE Evento (
    idEvento        INT             NOT NULL AUTO_INCREMENT,
    nome            VARCHAR(200)    NOT NULL,
    descricao       TEXT,
    dataEvento      DATE            NOT NULL,
    horario         TIME            NOT NULL,
    capacidade      INT             NOT NULL CHECK (capacidade > 0),
    status          VARCHAR(50)     NOT NULL DEFAULT 'Ativo',
    idOrganizador   INT             NOT NULL,
    idCategoria     INT             NOT NULL,
    idLocal         INT             NOT NULL,
    CONSTRAINT pk_evento       PRIMARY KEY (idEvento),
    CONSTRAINT fk_evento_org   FOREIGN KEY (idOrganizador) REFERENCES Organizador(idOrganizador),
    CONSTRAINT fk_evento_cat   FOREIGN KEY (idCategoria)   REFERENCES Categoria(idCategoria),
    CONSTRAINT fk_evento_local FOREIGN KEY (idLocal)       REFERENCES Local(idLocal)
);

-- ------------------------------------------------------------
-- 6. INGRESSO
-- Depende de: Participante, Evento
-- ------------------------------------------------------------
CREATE TABLE Ingresso (
    idIngresso      INT             NOT NULL AUTO_INCREMENT,
    tipoIngresso    VARCHAR(100)    NOT NULL,
    dataInscricao   DATE            NOT NULL,
    statusInscricao VARCHAR(50)     NOT NULL DEFAULT 'Ativo',
    quantidade      INT             NOT NULL DEFAULT 1 CHECK (quantidade > 0),
    valor           DECIMAL(10,2)   NOT NULL CHECK (valor >= 0),
    idParticipante  INT             NOT NULL,
    idEvento        INT             NOT NULL,
    CONSTRAINT pk_ingresso        PRIMARY KEY (idIngresso),
    CONSTRAINT fk_ingresso_part   FOREIGN KEY (idParticipante) REFERENCES Participante(idParticipante),
    CONSTRAINT fk_ingresso_evento FOREIGN KEY (idEvento)       REFERENCES Evento(idEvento),
    CONSTRAINT uq_ingresso        UNIQUE (idParticipante, idEvento)
);

-- ------------------------------------------------------------
-- 7. PAGAMENTO
-- Depende de: Ingresso
-- ------------------------------------------------------------
CREATE TABLE Pagamento (
    idPagamento     INT             NOT NULL AUTO_INCREMENT,
    dataPagamento   DATE            NOT NULL,
    valor_pago      DECIMAL(10,2)   NOT NULL CHECK (valor_pago >= 0),
    formaPagamento  VARCHAR(50)     NOT NULL,
    statusPagamento VARCHAR(50)     NOT NULL DEFAULT 'Pendente',
    idIngresso      INT             NOT NULL,
    CONSTRAINT pk_pagamento      PRIMARY KEY (idPagamento),
    CONSTRAINT fk_pagamento_ingr FOREIGN KEY (idIngresso) REFERENCES Ingresso(idIngresso)
);

-- ============================================================
--  Fim do script de criação
-- ============================================================
