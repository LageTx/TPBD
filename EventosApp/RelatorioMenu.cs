using MySql.Data.MySqlClient;

namespace EventosApp
{
    public static class RelatorioMenu
    {
        public static void Exibir()
        {
            while (true)
            {
                int op = Console2.Menu("RELATÓRIOS E CONSULTAS", new[]
                {
                    "Eventos com detalhes completos (JOIN)",
                    "Ingressos vendidos com pagamentos (JOIN)",
                    "Todos os usuários do sistema (UNION)",
                    "Participantes sem ingressos (DIFERENÇA)",
                    "Receita e ingressos por evento (COUNT + SUM)",
                    "Média de valores por categoria (AVG + HAVING)",
                    "Eventos com ocupação acima de 5 ingressos (GROUP BY + HAVING)",
                    "Arrecadação por forma de pagamento (SUM + COUNT)"
                });

                switch (op)
                {
                    case 1: EventosCompletos(); break;
                    case 2: IngressosComPagamentos(); break;
                    case 3: TodosUsuarios(); break;
                    case 4: ParticipantesSemIngresso(); break;
                    case 5: ReceitaPorEvento(); break;
                    case 6: MediaPorCategoria(); break;
                    case 7: OcupacaoEventos(); break;
                    case 8: ArrecadacaoPorFormaPagamento(); break;
                    case 0: return;
                }
            }
        }

        private static void EventosCompletos()
        {
            Console2.Titulo("EVENTOS COM DETALHES (JOIN)");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT E.idEvento, E.nome, E.dataEvento, E.horario, E.capacidade, E.status,
                             O.nome AS organizador, C.nomeCategoria, L.nomeLocal, L.cidade
                      FROM Evento E
                      INNER JOIN Organizador O ON E.idOrganizador = O.idOrganizador
                      INNER JOIN Categoria   C ON E.idCategoria   = C.idCategoria
                      INNER JOIN Local       L ON E.idLocal       = L.idLocal
                      ORDER BY E.dataEvento", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-4} {"Evento",-25} {"Data",-12} {"Status",-10} {"Organizador",-22} {"Categoria",-15} {"Local",-20} {"Cidade"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    var data = Convert.ToDateTime(reader["dataEvento"]).ToString("dd/MM/yyyy");
                    Console2.Info($"{reader["idEvento"],-4} {reader["nome"],-25} {data,-12} {reader["status"],-10} {reader["organizador"],-22} {reader["nomeCategoria"],-15} {reader["nomeLocal"],-20} {reader["cidade"]}");
                }
                if (!tem) Console2.Info("Nenhum evento cadastrado.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void IngressosComPagamentos()
        {
            Console2.Titulo("INGRESSOS VENDIDOS COM PAGAMENTOS (JOIN)");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT I.idIngresso, P.nome AS participante, E.nome AS evento,
                             I.tipoIngresso, I.valor, I.statusInscricao,
                             PG.formaPagamento, PG.valor_pago, PG.statusPagamento
                      FROM Ingresso I
                      INNER JOIN Participante P  ON I.idParticipante = P.idParticipante
                      INNER JOIN Evento       E  ON I.idEvento       = E.idEvento
                      LEFT  JOIN Pagamento    PG ON I.idIngresso     = PG.idIngresso
                      ORDER BY E.dataEvento, P.nome", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-4} {"Participante",-25} {"Evento",-25} {"Tipo",-10} {"Val.Ingr",-10} {"St.Ingr",-10} {"Forma Pgto",-18} {"Val.Pago",-10} {"St.Pgto"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    var forma  = reader["formaPagamento"] == DBNull.Value ? "-" : reader["formaPagamento"].ToString();
                    var valPago = reader["valor_pago"] == DBNull.Value ? "-" : $"R${Convert.ToDecimal(reader["valor_pago"]):F2}";
                    var stPgto = reader["statusPagamento"] == DBNull.Value ? "Pendente" : reader["statusPagamento"].ToString();
                    Console2.Info($"{reader["idIngresso"],-4} {reader["participante"],-25} {reader["evento"],-25} {reader["tipoIngresso"],-10} R${Convert.ToDecimal(reader["valor"]):F2,-8} {reader["statusInscricao"],-10} {forma,-18} {valPago,-10} {stPgto}");
                }
                if (!tem) Console2.Info("Nenhum ingresso encontrado.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void TodosUsuarios()
        {
            Console2.Titulo("TODOS OS USUÁRIOS DO SISTEMA (UNION)");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT email, nome, 'Organizador' AS perfil FROM Organizador
                      UNION
                      SELECT email, nome, 'Participante' AS perfil FROM Participante
                      ORDER BY nome", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"Nome",-30} {"E-mail",-35} {"Perfil"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    Console2.Info($"{reader["nome"],-30} {reader["email"],-35} {reader["perfil"]}");
                }
                if (!tem) Console2.Info("Nenhum usuário cadastrado.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void ParticipantesSemIngresso()
        {
            Console2.Titulo("PARTICIPANTES SEM INGRESSO (DIFERENÇA)");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT P.idParticipante, P.nome, P.email, P.telefone
                      FROM Participante P
                      WHERE P.idParticipante NOT IN (SELECT I.idParticipante FROM Ingresso I)
                      ORDER BY P.nome", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Nome",-30} {"E-mail",-35} {"Telefone"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    Console2.Info($"{reader["idParticipante"],-5} {reader["nome"],-30} {reader["email"],-35} {(reader["telefone"] == DBNull.Value ? "-" : reader["telefone"])}");
                }
                if (!tem) Console2.Info("Todos os participantes possuem pelo menos um ingresso.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void ReceitaPorEvento()
        {
            Console2.Titulo("RECEITA E INGRESSOS POR EVENTO (COUNT + SUM)");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT E.nome AS evento, E.dataEvento,
                             COUNT(I.idIngresso) AS totalVendidos,
                             COALESCE(SUM(I.valor), 0) AS receitaTotal
                      FROM Evento E
                      LEFT JOIN Ingresso I ON E.idEvento = I.idEvento
                      GROUP BY E.idEvento, E.nome, E.dataEvento
                      ORDER BY receitaTotal DESC", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"Evento",-30} {"Data",-12} {"Ingressos",-12} {"Receita Total"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    var data = Convert.ToDateTime(reader["dataEvento"]).ToString("dd/MM/yyyy");
                    Console2.Info($"{reader["evento"],-30} {data,-12} {reader["totalVendidos"],-12} R${Convert.ToDecimal(reader["receitaTotal"]):F2}");
                }
                if (!tem) Console2.Info("Nenhum evento cadastrado.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void MediaPorCategoria()
        {
            Console2.Titulo("MÉDIA DE VALORES POR CATEGORIA (AVG + HAVING)");
            Console2.Info("Exibe categorias com média de ingresso acima de R$50,00");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT C.nomeCategoria, COUNT(I.idIngresso) AS totalIngressos,
                             ROUND(AVG(I.valor),2) AS mediaValor,
                             MAX(I.valor) AS maiorValor, MIN(I.valor) AS menorValor
                      FROM Categoria C
                      INNER JOIN Evento   E ON C.idCategoria = E.idCategoria
                      INNER JOIN Ingresso I ON E.idEvento    = I.idEvento
                      GROUP BY C.idCategoria, C.nomeCategoria
                      HAVING AVG(I.valor) > 50.00
                      ORDER BY mediaValor DESC", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"Categoria",-25} {"Total",-8} {"Média",-12} {"Maior",-12} {"Menor"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    Console2.Info($"{reader["nomeCategoria"],-25} {reader["totalIngressos"],-8} R${Convert.ToDecimal(reader["mediaValor"]):F2,-10} R${Convert.ToDecimal(reader["maiorValor"]):F2,-10} R${Convert.ToDecimal(reader["menorValor"]):F2}");
                }
                if (!tem) Console2.Info("Nenhuma categoria com média acima de R$50,00.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void OcupacaoEventos()
        {
            Console2.Titulo("OCUPAÇÃO DOS EVENTOS (GROUP BY + HAVING)");
            Console2.Info("Exibe eventos com mais de 5 ingressos vendidos");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT E.nome AS evento, E.capacidade,
                             COUNT(I.idIngresso) AS vendidos,
                             ROUND(COUNT(I.idIngresso) / E.capacidade * 100, 2) AS ocupacao
                      FROM Evento E
                      INNER JOIN Ingresso I ON E.idEvento = I.idEvento
                      GROUP BY E.idEvento, E.nome, E.capacidade
                      HAVING COUNT(I.idIngresso) > 5
                      ORDER BY ocupacao DESC", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"Evento",-30} {"Capacidade",-12} {"Vendidos",-10} {"Ocupação"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    Console2.Info($"{reader["evento"],-30} {reader["capacidade"],-12} {reader["vendidos"],-10} {reader["ocupacao"]}%");
                }
                if (!tem) Console2.Info("Nenhum evento com mais de 5 ingressos vendidos.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void ArrecadacaoPorFormaPagamento()
        {
            Console2.Titulo("ARRECADAÇÃO POR FORMA DE PAGAMENTO (SUM + COUNT)");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT PG.formaPagamento,
                             COUNT(PG.idPagamento) AS quantidade,
                             SUM(PG.valor_pago) AS totalArrecadado
                      FROM Pagamento PG
                      INNER JOIN Ingresso I ON PG.idIngresso = I.idIngresso
                      WHERE PG.statusPagamento = 'Aprovado'
                      GROUP BY PG.formaPagamento
                      ORDER BY totalArrecadado DESC", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"Forma de Pagamento",-25} {"Qtd Pagamentos",-17} {"Total Arrecadado"}");
                Console2.Linha();

                bool tem = false;
                decimal grandTotal = 0;
                while (reader.Read())
                {
                    tem = true;
                    var total = Convert.ToDecimal(reader["totalArrecadado"]);
                    grandTotal += total;
                    Console2.Info($"{reader["formaPagamento"],-25} {reader["quantidade"],-17} R${total:F2}");
                }
                if (!tem)
                    Console2.Info("Nenhum pagamento aprovado encontrado.");
                else
                {
                    Console2.Linha();
                    Console.WriteLine($"\n  Total geral: R${grandTotal:F2}");
                }
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }
    }
}
