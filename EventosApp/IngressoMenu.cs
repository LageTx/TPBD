using MySql.Data.MySqlClient;

namespace EventosApp
{
    public static class IngressoMenu
    {
        public static void Exibir()
        {
            while (true)
            {
                int op = Console2.Menu("VENDA DE INGRESSOS", new[]
                {
                    "Vender ingresso",
                    "Listar ingressos por evento",
                    "Cancelar ingresso"
                });

                switch (op)
                {
                    case 1: Vender(); break;
                    case 2: ListarPorEvento(); break;
                    case 3: Cancelar(); break;
                    case 0: return;
                }
            }
        }

        private static void Vender()
        {
            Console2.Titulo("VENDER INGRESSO");

            Console2.Subtitulo("Eventos disponíveis (status: Ativo)");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                // Lista eventos com vagas disponíveis
                var listCmd = new MySqlCommand(
                    @"SELECT E.idEvento, E.nome, E.dataEvento, E.capacidade,
                             (E.capacidade - COUNT(I.idIngresso)) AS vagas
                      FROM Evento E
                      LEFT JOIN Ingresso I ON E.idEvento = I.idEvento AND I.statusInscricao = 'Ativo'
                      WHERE E.status = 'Ativo'
                      GROUP BY E.idEvento, E.nome, E.dataEvento, E.capacidade
                      HAVING vagas > 0
                      ORDER BY E.dataEvento", conn);
                using var reader = listCmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Nome",-30} {"Data",-12} {"Vagas disponíveis",-20}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    var data = Convert.ToDateTime(reader["dataEvento"]).ToString("dd/MM/yyyy");
                    Console2.Info($"{reader["idEvento"],-5} {reader["nome"],-30} {data,-12} {reader["vagas"],-20}");
                }
                if (!tem)
                {
                    Console2.Info("Nenhum evento com vagas disponíveis.");
                    Console2.PressEnter();
                    return;
                }
                reader.Close();

                var idEvento = Console2.LerInteiro("ID do evento", 1);

                // Verifica vagas novamente antes de prosseguir
                var vagasCmd = new MySqlCommand(
                    @"SELECT (E.capacidade - COUNT(I.idIngresso)) AS vagas
                      FROM Evento E
                      LEFT JOIN Ingresso I ON E.idEvento = I.idEvento AND I.statusInscricao = 'Ativo'
                      WHERE E.idEvento = @id
                      GROUP BY E.idEvento, E.capacidade", conn);
                vagasCmd.Parameters.AddWithValue("@id", idEvento);
                var vagasResult = vagasCmd.ExecuteScalar();
                if (vagasResult == null || Convert.ToInt32(vagasResult) <= 0)
                {
                    Console2.Erro("Evento sem vagas disponíveis.");
                    Console2.PressEnter();
                    return;
                }

                Console2.Subtitulo("Participantes disponíveis");
                var partCmd = new MySqlCommand("SELECT idParticipante, nome, cpf FROM Participante ORDER BY nome", conn);
                using var partReader = partCmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Nome",-30} {"CPF",-13}");
                Console2.Linha();
                while (partReader.Read())
                    Console2.Info($"{partReader["idParticipante"],-5} {partReader["nome"],-30} {partReader["cpf"],-13}");
                partReader.Close();

                var idParticipante = Console2.LerInteiro("ID do participante", 1);

                // Verifica se já tem ingresso ativo nesse evento
                var dupCmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM Ingresso WHERE idParticipante=@p AND idEvento=@e AND statusInscricao='Ativo'", conn);
                dupCmd.Parameters.AddWithValue("@p", idParticipante);
                dupCmd.Parameters.AddWithValue("@e", idEvento);
                if (Convert.ToInt32(dupCmd.ExecuteScalar()) > 0)
                {
                    Console2.Erro("Este participante já possui um ingresso ativo para este evento.");
                    Console2.PressEnter();
                    return;
                }

                Console2.Subtitulo("Dados do ingresso");
                var tipo = Console2.LerTexto("Tipo do ingresso (ex: Inteira, Meia, VIP)");
                var valor = Console2.LerDecimal("Valor (R$)", 0);

                var cmd = new MySqlCommand(
                    @"INSERT INTO Ingresso (tipoIngresso, dataInscricao, statusInscricao, quantidade, valor, idParticipante, idEvento)
                      VALUES (@tipo, CURDATE(), 'Ativo', 1, @valor, @part, @evento)", conn);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.Parameters.AddWithValue("@part", idParticipante);
                cmd.Parameters.AddWithValue("@evento", idEvento);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Ingresso vendido com sucesso!");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        public static void ListarPorEvento()
        {
            Console2.Titulo("INGRESSOS POR EVENTO");
            EventoMenu.Listar();
            var idEvento = Console2.LerInteiro("ID do evento", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT I.idIngresso, P.nome AS participante, P.cpf, I.tipoIngresso,
                             I.valor, I.dataInscricao, I.statusInscricao
                      FROM Ingresso I
                      INNER JOIN Participante P ON I.idParticipante = P.idParticipante
                      WHERE I.idEvento = @id
                      ORDER BY I.dataInscricao", conn);
                cmd.Parameters.AddWithValue("@id", idEvento);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Participante",-28} {"CPF",-13} {"Tipo",-12} {"Valor",-10} {"Data",-12} {"Status"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    var data = Convert.ToDateTime(reader["dataInscricao"]).ToString("dd/MM/yyyy");
                    Console2.Info($"{reader["idIngresso"],-5} {reader["participante"],-28} {reader["cpf"],-13} {reader["tipoIngresso"],-12} R${Convert.ToDecimal(reader["valor"]):F2,-8} {data,-12} {reader["statusInscricao"]}");
                }
                if (!tem) Console2.Info("Nenhum ingresso encontrado para este evento.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void Cancelar()
        {
            Console2.Titulo("CANCELAR INGRESSO");
            EventoMenu.Listar();
            var idEvento = Console2.LerInteiro("ID do evento", 1);
            ListarPorEvento();
            var idIngresso = Console2.LerInteiro("ID do ingresso a cancelar", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                var check = new MySqlCommand(
                    "SELECT COUNT(*) FROM Ingresso WHERE idIngresso=@id AND statusInscricao='Ativo'", conn);
                check.Parameters.AddWithValue("@id", idIngresso);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    Console2.Erro("Ingresso não encontrado ou já cancelado.");
                    Console2.PressEnter();
                    return;
                }

                Console.Write("\n  Confirma cancelamento do ingresso? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s") { Console2.Info("Cancelado."); Console2.PressEnter(); return; }

                var cmd = new MySqlCommand(
                    "UPDATE Ingresso SET statusInscricao='Cancelado' WHERE idIngresso=@id", conn);
                cmd.Parameters.AddWithValue("@id", idIngresso);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Ingresso cancelado com sucesso!");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }
    }
}
