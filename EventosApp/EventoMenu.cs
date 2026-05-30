using MySql.Data.MySqlClient;

namespace EventosApp
{
    public static class EventoMenu
    {
        public static void Exibir()
        {
            while (true)
            {
                int op = Console2.Menu("EVENTOS", new[]
                {
                    "Cadastrar evento",
                    "Listar eventos",
                    "Editar evento",
                    "Excluir evento"
                });

                switch (op)
                {
                    case 1: Cadastrar(); break;
                    case 2: Listar(); break;
                    case 3: Editar(); break;
                    case 4: Excluir(); break;
                    case 0: return;
                }
            }
        }

        private static void Cadastrar()
        {
            Console2.Titulo("CADASTRAR EVENTO");

            // Exibe dependências antes de pedir os dados
            Console2.Subtitulo("Organizadores disponíveis");
            OrganizadorMenu.Listar();
            var idOrg = Console2.LerInteiro("ID do organizador", 1);

            Console2.Subtitulo("Categorias disponíveis");
            CategoriaLocalMenu.ListarCategorias();
            var idCat = Console2.LerInteiro("ID da categoria", 1);

            Console2.Subtitulo("Locais disponíveis");
            CategoriaLocalMenu.ListarLocais();
            var idLocal = Console2.LerInteiro("ID do local", 1);

            Console2.Subtitulo("Dados do evento");
            var nome = Console2.LerTexto("Nome do evento");
            var descricao = Console2.LerTexto("Descrição", obrigatorio: false);
            var data = Console2.LerData("Data do evento");
            var hora = Console2.LerHora("Horário");
            var capacidade = Console2.LerInteiro("Capacidade máxima", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"INSERT INTO Evento (nome, descricao, dataEvento, horario, capacidade, status, idOrganizador, idCategoria, idLocal)
                      VALUES (@nome,@desc,@data,@hora,@cap,'Ativo',@org,@cat,@local)", conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@desc", descricao == "" ? DBNull.Value : (object)descricao);
                cmd.Parameters.AddWithValue("@data", data.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@hora", hora.ToString(@"hh\:mm\:ss"));
                cmd.Parameters.AddWithValue("@cap", capacidade);
                cmd.Parameters.AddWithValue("@org", idOrg);
                cmd.Parameters.AddWithValue("@cat", idCat);
                cmd.Parameters.AddWithValue("@local", idLocal);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Evento cadastrado com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1452)
            {
                Console2.Erro("ID de organizador, categoria ou local não encontrado.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        public static void Listar()
        {
            Console2.Titulo("LISTA DE EVENTOS");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT E.idEvento, E.nome, E.dataEvento, E.horario, E.capacidade, E.status,
                             O.nome AS organizador, C.nomeCategoria, L.nomeLocal
                      FROM Evento E
                      INNER JOIN Organizador O ON E.idOrganizador = O.idOrganizador
                      INNER JOIN Categoria   C ON E.idCategoria   = C.idCategoria
                      INNER JOIN Local       L ON E.idLocal       = L.idLocal
                      ORDER BY E.dataEvento", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Nome",-28} {"Data",-12} {"Hora",-7} {"Cap.",-6} {"Status",-12} {"Organizador",-22} {"Categoria",-18} {"Local"}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    var data = Convert.ToDateTime(reader["dataEvento"]).ToString("dd/MM/yyyy");
                    var hora = reader["horario"].ToString()![..5];
                    Console2.Info($"{reader["idEvento"],-5} {reader["nome"],-28} {data,-12} {hora,-7} {reader["capacidade"],-6} {reader["status"],-12} {reader["organizador"],-22} {reader["nomeCategoria"],-18} {reader["nomeLocal"]}");
                }
                if (!tem) Console2.Info("Nenhum evento cadastrado.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void Editar()
        {
            Console2.Titulo("EDITAR EVENTO");
            Listar();
            var id = Console2.LerInteiro("ID do evento a editar", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                var check = new MySqlCommand("SELECT COUNT(*) FROM Evento WHERE idEvento=@id", conn);
                check.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    Console2.Erro("Evento não encontrado.");
                    Console2.PressEnter();
                    return;
                }

                var nome = Console2.LerTexto("Novo nome");
                var descricao = Console2.LerTexto("Nova descrição", obrigatorio: false);
                var data = Console2.LerData("Nova data");
                var hora = Console2.LerHora("Novo horário");
                var capacidade = Console2.LerInteiro("Nova capacidade", 1);

                Console2.Subtitulo("Status disponíveis: Ativo / Cancelado / Encerrado");
                var status = Console2.LerTexto("Novo status");

                var cmd = new MySqlCommand(
                    @"UPDATE Evento SET nome=@nome, descricao=@desc, dataEvento=@data,
                      horario=@hora, capacidade=@cap, status=@status WHERE idEvento=@id", conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@desc", descricao == "" ? DBNull.Value : (object)descricao);
                cmd.Parameters.AddWithValue("@data", data.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@hora", hora.ToString(@"hh\:mm\:ss"));
                cmd.Parameters.AddWithValue("@cap", capacidade);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Evento atualizado com sucesso!");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void Excluir()
        {
            Console2.Titulo("EXCLUIR EVENTO");
            Listar();
            var id = Console2.LerInteiro("ID do evento a excluir", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                var check = new MySqlCommand("SELECT COUNT(*) FROM Evento WHERE idEvento=@id", conn);
                check.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    Console2.Erro("Evento não encontrado.");
                    Console2.PressEnter();
                    return;
                }

                Console.Write("\n  Confirma exclusão? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s") { Console2.Info("Cancelado."); Console2.PressEnter(); return; }

                var cmd = new MySqlCommand("DELETE FROM Evento WHERE idEvento=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Evento excluído com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                Console2.Erro("Não é possível excluir: evento possui ingressos vinculados.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }
    }
}
