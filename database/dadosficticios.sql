-- ============================================================
-- CATEGORIA
-- ============================================================

INSERT INTO Categoria (nomeCategoria, descricao) VALUES
('Tecnologia', 'Eventos relacionados à tecnologia e inovação'),
('Negócios', 'Eventos corporativos e empresariais'),
('Educação', 'Palestras e workshops educacionais'),
('Saúde', 'Eventos voltados para saúde e bem-estar'),
('Entretenimento', 'Shows, festivais e atividades culturais');

-- ============================================================
-- LOCAL
-- ============================================================

INSERT INTO Local (nomeLocal, endereco, cidade, capacidade) VALUES
('Centro de Convenções BH', 'Av. Amazonas, 1000', 'Belo Horizonte', 1500),
('Auditório PUC Minas', 'Rua Dom José Gaspar, 500', 'Belo Horizonte', 500),
('Expominas', 'Av. Amazonas, 6200', 'Belo Horizonte', 3000),
('Teatro Municipal', 'Praça Central, 50', 'Contagem', 800),
('Arena Eventos', 'Av. Principal, 1200', 'Betim', 2000);

-- ============================================================
-- ORGANIZADOR
-- ============================================================

INSERT INTO Organizador (nome, email, telefone) VALUES
('João Martins', 'joao.martins@email.com', '31999990001'),
('Ana Souza', 'ana.souza@email.com', '31999990002'),
('Carlos Ferreira', 'carlos.ferreira@email.com', '31999990003'),
('Mariana Lima', 'mariana.lima@email.com', '31999990004'),
('Pedro Almeida', 'pedro.almeida@email.com', '31999990005');

-- ============================================================
-- PARTICIPANTE
-- ============================================================

INSERT INTO Participante (nome, cpf, email, telefone, dataNascimento) VALUES
('Lucas Silva', '12345678901', 'lucas@email.com', '31988880001', '1998-03-15'),
('Fernanda Costa', '12345678902', 'fernanda@email.com', '31988880002', '2000-07-22'),
('Rafael Oliveira', '12345678903', 'rafael@email.com', '31988880003', '1995-11-10'),
('Juliana Rocha', '12345678904', 'juliana@email.com', '31988880004', '2001-05-30'),
('Bruno Santos', '12345678905', 'bruno@email.com', '31988880005', '1997-08-14'),
('Camila Alves', '12345678906', 'camila@email.com', '31988880006', '1999-09-09'),
('Gabriel Mendes', '12345678907', 'gabriel@email.com', '31988880007', '2002-01-20'),
('Larissa Gomes', '12345678908', 'larissa@email.com', '31988880008', '1996-12-18'),
('Thiago Pereira', '12345678909', 'thiago@email.com', '31988880009', '1994-06-05'),
('Patricia Ribeiro', '12345678910', 'patricia@email.com', '31988880010', '1998-10-27');

-- ============================================================
-- EVENTO
-- ============================================================

INSERT INTO Evento
(nome, descricao, dataEvento, horario, capacidade, status,
 idOrganizador, idCategoria, idLocal)
VALUES
('Tech Summit 2026',
 'Congresso de tecnologia e inovação',
 '2026-08-15',
 '09:00:00',
 1000,
 'Ativo',
 1, 1, 1),

('Fórum de Negócios',
 'Networking e empreendedorismo',
 '2026-09-20',
 '14:00:00',
 600,
 'Ativo',
 2, 2, 2),

('Semana Acadêmica ADS',
 'Palestras e minicursos',
 '2026-10-05',
 '19:00:00',
 450,
 'Ativo',
 3, 3, 2),

('Congresso Saúde Integral',
 'Tendências em saúde e qualidade de vida',
 '2026-11-12',
 '08:00:00',
 1200,
 'Ativo',
 4, 4, 3),

('Festival Cultural Minas',
 'Música, arte e gastronomia',
 '2026-12-10',
 '18:00:00',
 1800,
 'Ativo',
 5, 5, 5);

-- ============================================================
-- INGRESSO
-- ============================================================

INSERT INTO Ingresso
(tipoIngresso, dataInscricao, statusInscricao,
 quantidade, valor, idParticipante, idEvento)
VALUES
('Inteira', '2026-07-01', 'Ativo', 1, 150.00, 1, 1),
('Meia', '2026-07-02', 'Ativo', 1, 75.00, 2, 1),
('VIP', '2026-08-01', 'Ativo', 1, 300.00, 3, 1),
('Inteira', '2026-08-15', 'Ativo', 1, 120.00, 4, 2),
('Inteira', '2026-08-16', 'Ativo', 1, 120.00, 5, 2),
('Meia', '2026-09-01', 'Ativo', 1, 50.00, 6, 3),
('Meia', '2026-09-02', 'Ativo', 1, 50.00, 7, 3),
('Inteira', '2026-10-01', 'Ativo', 1, 200.00, 8, 4),
('VIP', '2026-10-02', 'Ativo', 1, 400.00, 9, 4),
('Inteira', '2026-11-01', 'Ativo', 1, 180.00, 10, 5);

-- ============================================================
-- PAGAMENTO
-- ============================================================

INSERT INTO Pagamento
(dataPagamento, valor_pago, formaPagamento,
 statusPagamento, idIngresso)
VALUES
('2026-07-01', 150.00, 'Cartão de Crédito', 'Pago', 1),
('2026-07-02', 75.00, 'PIX', 'Pago', 2),
('2026-08-01', 300.00, 'Cartão de Crédito', 'Pago', 3),
('2026-08-15', 120.00, 'Boleto', 'Pago', 4),
('2026-08-16', 120.00, 'PIX', 'Pago', 5),
('2026-09-01', 50.00, 'PIX', 'Pago', 6),
('2026-09-02', 50.00, 'Cartão de Débito', 'Pago', 7),
('2026-10-01', 200.00, 'Cartão de Crédito', 'Pago', 8),
('2026-10-02', 400.00, 'PIX', 'Pago', 9),
('2026-11-01', 180.00, 'Boleto', 'Pendente', 10);