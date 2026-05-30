using MySql.Data.MySqlClient;

namespace EventosApp
{
    public static class PagamentoMenu
    {
        public static void Exibir()
        {
            while (true)
            {
                int op = Console2.Menu("PAGAMENTOS", new[]
                {
                    "Registrar pagamento",
                    "Listar pagamentos por evento",
                    "Listar pagamentos pendentes"
                });

                switch (op)
                {
                    case 1: Registrar(); break;
                    case 2: ListarPorEvento(); break;
                    case 3: ListarPendentes(); break;
                    case 0: return;
                }
            }
        }

        private static void Registrar()
        {
            Console2.Titulo("REGISTRAR PAGAMENTO");

            Console2.Subtitulo("Ingressos sem pagamento registrado");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                var listCmd = new MySqlCommand(
                    @"SELECT I.idIngresso, P.nome AS participante, E.nome AS evento,
                             I.tipoIngresso, I.valor
                      FROM Ingresso I
                      INNER JOIN Participante P ON I.idParticipante = P.idParticipante
                      INNER JOIN Evento       E ON I.idEvento       = E.idEvento
                      WHERE I.statusInscricao = 'Ativo'
                        AND I.idIngresso NOT IN (SELECT idIngresso FROM Pagamento)
                      ORDER BY E.dataEvento, P.nome", conn);
                using var reader = listCmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Participante",-28} {"Evento",-28} {"Tipo",-12} {"Valor"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    Console2.Info($"{reader["idIngresso"],-5} {reader["participante"],-28} {reader["evento"],-28} {reader["tipoIngresso"],-12} R${Convert.ToDecimal(reader["valor"]):F2}");
                }
                if (!tem)
                {
                    Console2.Info("Nenhum ingresso aguardando pagamento.");
                    Console2.PressEnter();
                    return;
                }
                reader.Close();

                var idIngresso = Console2.LerInteiro("ID do ingresso", 1);

                // Busca o valor do ingresso como sugestão
                var valorCmd = new MySqlCommand("SELECT valor FROM Ingresso WHERE idIngresso=@id", conn);
                valorCmd.Parameters.AddWithValue("@id", idIngresso);
                var valorSugerido = Convert.ToDecimal(valorCmd.ExecuteScalar() ?? 0);
                Console2.Info($"Valor sugerido: R${valorSugerido:F2}");

                var valorPago = Console2.LerDecimal("Valor pago (R$)", 0);

                Console2.Subtitulo("Formas de pagamento: Dinheiro / Cartão de Crédito / Cartão de Débito / PIX");
                var forma = Console2.LerTexto("Forma de pagamento");

                var cmd = new MySqlCommand(
                    @"INSERT INTO Pagamento (dataPagamento, valor_pago, formaPagamento, statusPagamento, idIngresso)
                      VALUES (CURDATE(), @valor, @forma, 'Aprovado', @ingresso)", conn);
                cmd.Parameters.AddWithValue("@valor", valorPago);
                cmd.Parameters.AddWithValue("@forma", forma);
                cmd.Parameters.AddWithValue("@ingresso", idIngresso);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Pagamento registrado com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1452)
            {
                Console2.Erro("Ingresso não encontrado.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void ListarPorEvento()
        {
            Console2.Titulo("PAGAMENTOS POR EVENTO");
            EventoMenu.Listar();
            var idEvento = Console2.LerInteiro("ID do evento", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT PG.idPagamento, P.nome AS participante, I.tipoIngresso,
                             PG.valor_pago, PG.formaPagamento, PG.dataPagamento, PG.statusPagamento
                      FROM Pagamento PG
                      INNER JOIN Ingresso    I  ON PG.idIngresso    = I.idIngresso
                      INNER JOIN Participante P ON I.idParticipante = P.idParticipante
                      WHERE I.idEvento = @id
                      ORDER BY PG.dataPagamento", conn);
                cmd.Parameters.AddWithValue("@id", idEvento);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Participante",-28} {"Tipo",-12} {"Valor",-10} {"Forma",-20} {"Data",-12} {"Status"}");
                Console2.Linha();

                bool tem = false;
                decimal total = 0;
                while (reader.Read())
                {
                    tem = true;
                    var valor = Convert.ToDecimal(reader["valor_pago"]);
                    total += valor;
                    var data = Convert.ToDateTime(reader["dataPagamento"]).ToString("dd/MM/yyyy");
                    Console2.Info($"{reader["idPagamento"],-5} {reader["participante"],-28} {reader["tipoIngresso"],-12} R${valor,-8:F2} {reader["formaPagamento"],-20} {data,-12} {reader["statusPagamento"]}");
                }
                if (!tem)
                    Console2.Info("Nenhum pagamento encontrado para este evento.");
                else
                {
                    Console2.Linha();
                    Console.WriteLine($"\n  Total arrecadado: R${total:F2}");
                }
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void ListarPendentes()
        {
            Console2.Titulo("INGRESSOS SEM PAGAMENTO");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT I.idIngresso, P.nome AS participante, E.nome AS evento,
                             E.dataEvento, I.tipoIngresso, I.valor
                      FROM Ingresso I
                      INNER JOIN Participante P ON I.idParticipante = P.idParticipante
                      INNER JOIN Evento       E ON I.idEvento       = E.idEvento
                      WHERE I.statusInscricao = 'Ativo'
                        AND I.idIngresso NOT IN (SELECT idIngresso FROM Pagamento)
                      ORDER BY E.dataEvento", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Participante",-28} {"Evento",-28} {"Data",-12} {"Tipo",-12} {"Valor"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    var data = Convert.ToDateTime(reader["dataEvento"]).ToString("dd/MM/yyyy");
                    Console2.Info($"{reader["idIngresso"],-5} {reader["participante"],-28} {reader["evento"],-28} {data,-12} {reader["tipoIngresso"],-12} R${Convert.ToDecimal(reader["valor"]):F2}");
                }
                if (!tem) Console2.Info("Nenhum ingresso pendente de pagamento.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }
    }
}
