-- ============================================================
--  Sistema de Gerenciamento de Eventos
--  PUC Minas - Análise e Desenvolvimento de Sistemas
--  Parte 2 – Consultas em SQL
-- ============================================================


-- ============================================================
--  BLOCO 1 – CONSULTAS COM JUNÇÃO (JOIN)
-- ============================================================

-- ------------------------------------------------------------
-- Consulta J1: Listar todos os eventos com informações do
-- organizador, categoria e local onde serão realizados.
-- Envolve as tabelas: Evento, Organizador, Categoria e Local.
-- ------------------------------------------------------------
SELECT
    E.idEvento,
    E.nome              AS nomeEvento,
    E.dataEvento,
    E.horario,
    E.capacidade,
    E.status,
    O.nome              AS nomeOrganizador,
    O.email             AS emailOrganizador,
    C.nomeCategoria,
    L.nomeLocal,
    L.cidade
FROM Evento E
INNER JOIN Organizador O ON E.idOrganizador = O.idOrganizador
INNER JOIN Categoria   C ON E.idCategoria   = C.idCategoria
INNER JOIN Local       L ON E.idLocal       = L.idLocal
ORDER BY E.dataEvento;


-- ------------------------------------------------------------
-- Consulta J2: Listar todos os ingressos vendidos com os dados
-- do participante, do evento e do pagamento correspondente.
-- Envolve as tabelas: Ingresso, Participante, Evento e Pagamento.
-- ------------------------------------------------------------
SELECT
    I.idIngresso,
    P.nome              AS nomeParticipante,
    P.cpf,
    P.email,
    E.nome              AS nomeEvento,
    E.dataEvento,
    I.tipoIngresso,
    I.valor             AS valorIngresso,
    I.statusInscricao,
    PG.formaPagamento,
    PG.valor_pago,
    PG.statusPagamento
FROM Ingresso I
INNER JOIN Participante P  ON I.idParticipante = P.idParticipante
INNER JOIN Evento       E  ON I.idEvento       = E.idEvento
LEFT JOIN  Pagamento    PG ON I.idIngresso     = PG.idIngresso
ORDER BY E.dataEvento, P.nome;


-- ============================================================
--  BLOCO 2 – CONSULTAS COM OPERAÇÕES DE CONJUNTO
--  Observação: MySQL não possui INTERSECT nem EXCEPT nativos.
--  Utilizamos alternativas equivalentes com IN / NOT IN / JOIN.
-- ============================================================

-- ------------------------------------------------------------
-- Consulta C1 – UNIÃO (UNION)
-- Retorna uma lista unificada de e-mails cadastrados no sistema,
-- tanto de organizadores quanto de participantes, sem repetição.
-- Envolve as tabelas: Organizador e Participante.
-- ------------------------------------------------------------
SELECT email, nome, 'Organizador' AS perfil
FROM Organizador

UNION

SELECT email, nome, 'Participante' AS perfil
FROM Participante
ORDER BY nome;


-- ------------------------------------------------------------
-- Consulta C2 – INTERSEÇÃO (INTERSECT simulado com IN)
-- Retorna os e-mails que estão cadastrados tanto como
-- organizador quanto como participante no sistema.
-- Envolve as tabelas: Organizador e Participante.
-- ------------------------------------------------------------
SELECT O.email, O.nome, 'Organizador e Participante' AS perfil
FROM Organizador O
WHERE O.email IN (
    SELECT P.email
    FROM Participante P
);


-- ------------------------------------------------------------
-- Consulta C3 – DIFERENÇA (EXCEPT simulado com NOT IN)
-- Retorna os participantes que ainda não realizaram nenhuma
-- compra de ingresso, ou seja, estão cadastrados mas nunca
-- adquiriram um ingresso para nenhum evento.
-- Envolve as tabelas: Participante e Ingresso.
-- ------------------------------------------------------------
SELECT P.idParticipante, P.nome, P.email, P.telefone
FROM Participante P
WHERE P.idParticipante NOT IN (
    SELECT I.idParticipante
    FROM Ingresso I
)
ORDER BY P.nome;


-- ============================================================
--  BLOCO 3 – CONSULTAS COM AGREGAÇÃO
-- ============================================================

-- ------------------------------------------------------------
-- Consulta A1 – COUNT e SUM
-- Retorna o total de ingressos vendidos e a receita total
-- arrecadada por evento.
-- Envolve as tabelas: Evento e Ingresso.
-- ------------------------------------------------------------
SELECT
    E.nome                      AS nomeEvento,
    E.dataEvento,
    COUNT(I.idIngresso)         AS totalIngressosVendidos,
    SUM(I.valor)                AS receitaTotal
FROM Evento E
LEFT JOIN Ingresso I ON E.idEvento = I.idEvento
GROUP BY E.idEvento, E.nome, E.dataEvento
ORDER BY receitaTotal DESC;


-- ------------------------------------------------------------
-- Consulta A2 – AVG, MAX e MIN com GROUP BY e HAVING
-- Retorna a média, o maior e o menor valor de ingresso
-- por categoria, considerando apenas categorias com
-- média de valor acima de R$ 50,00.
-- Envolve as tabelas: Categoria, Evento e Ingresso.
-- ------------------------------------------------------------
SELECT
    C.nomeCategoria,
    COUNT(I.idIngresso)         AS totalIngressos,
    ROUND(AVG(I.valor), 2)     AS mediaValor,
    MAX(I.valor)                AS maiorValor,
    MIN(I.valor)                AS menorValor
FROM Categoria C
INNER JOIN Evento   E ON C.idCategoria = E.idCategoria
INNER JOIN Ingresso I ON E.idEvento    = I.idEvento
GROUP BY C.idCategoria, C.nomeCategoria
HAVING AVG(I.valor) > 50.00
ORDER BY mediaValor DESC;


-- ------------------------------------------------------------
-- Consulta A3 – COUNT com GROUP BY e HAVING
-- Retorna os eventos que possuem mais de 5 ingressos vendidos,
-- exibindo também o percentual de ocupação em relação à
-- capacidade total do evento.
-- Envolve as tabelas: Evento e Ingresso.
-- ------------------------------------------------------------
SELECT
    E.nome                                              AS nomeEvento,
    E.capacidade,
    COUNT(I.idIngresso)                                 AS ingressosVendidos,
    ROUND(COUNT(I.idIngresso) / E.capacidade * 100, 2) AS percentualOcupacao
FROM Evento E
INNER JOIN Ingresso I ON E.idEvento = I.idEvento
GROUP BY E.idEvento, E.nome, E.capacidade
HAVING COUNT(I.idIngresso) > 5
ORDER BY percentualOcupacao DESC;


-- ------------------------------------------------------------
-- Consulta A4 – SUM e COUNT sobre pagamentos
-- Retorna o total arrecadado e a quantidade de pagamentos
-- por forma de pagamento, considerando apenas pagamentos
-- com status 'Aprovado'.
-- Envolve as tabelas: Pagamento e Ingresso.
-- ------------------------------------------------------------
SELECT
    PG.formaPagamento,
    COUNT(PG.idPagamento)       AS quantidadePagamentos,
    SUM(PG.valor_pago)          AS totalArrecadado
FROM Pagamento PG
INNER JOIN Ingresso I ON PG.idIngresso = I.idIngresso
WHERE PG.statusPagamento = 'Aprovado'
GROUP BY PG.formaPagamento
ORDER BY totalArrecadado DESC;


-- ============================================================
--  Fim das consultas
-- ============================================================
