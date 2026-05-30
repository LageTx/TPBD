using MySql.Data.MySqlClient;

namespace EventosApp
{
    public static class CategoriaLocalMenu
    {
        public static void Exibir()
        {
            while (true)
            {
                int op = Console2.Menu("CATEGORIAS E LOCAIS", new[]
                {
                    "Cadastrar categoria",
                    "Listar categorias",
                    "Excluir categoria",
                    "Cadastrar local",
                    "Listar locais",
                    "Excluir local"
                });

                switch (op)
                {
                    case 1: CadastrarCategoria(); break;
                    case 2: ListarCategorias(); break;
                    case 3: ExcluirCategoria(); break;
                    case 4: CadastrarLocal(); break;
                    case 5: ListarLocais(); break;
                    case 6: ExcluirLocal(); break;
                    case 0: return;
                }
            }
        }

        // ── CATEGORIAS ──────────────────────────────────────────────

        private static void CadastrarCategoria()
        {
            Console2.Titulo("CADASTRAR CATEGORIA");
            var nome = Console2.LerTexto("Nome da categoria");
            var descricao = Console2.LerTexto("Descrição", obrigatorio: false);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO Categoria (nomeCategoria, descricao) VALUES (@nome, @desc)", conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@desc", descricao == "" ? DBNull.Value : (object)descricao);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Categoria cadastrada com sucesso!");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        public static void ListarCategorias()
        {
            Console2.Titulo("LISTA DE CATEGORIAS");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand("SELECT idCategoria, nomeCategoria, descricao FROM Categoria ORDER BY nomeCategoria", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Nome",-30} {"Descrição",-40}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    Console2.Info($"{reader["idCategoria"],-5} {reader["nomeCategoria"],-30} {(reader["descricao"] == DBNull.Value ? "-" : reader["descricao"]),-40}");
                }
                if (!tem) Console2.Info("Nenhuma categoria cadastrada.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void ExcluirCategoria()
        {
            Console2.Titulo("EXCLUIR CATEGORIA");
            ListarCategorias();
            var id = Console2.LerInteiro("ID da categoria a excluir", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                Console.Write("\n  Confirma exclusão? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s") { Console2.Info("Cancelado."); Console2.PressEnter(); return; }

                var cmd = new MySqlCommand("DELETE FROM Categoria WHERE idCategoria=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Categoria excluída com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                Console2.Erro("Não é possível excluir: categoria possui eventos vinculados.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        // ── LOCAIS ──────────────────────────────────────────────────

        private static void CadastrarLocal()
        {
            Console2.Titulo("CADASTRAR LOCAL");
            var nome = Console2.LerTexto("Nome do local");
            var endereco = Console2.LerTexto("Endereço");
            var cidade = Console2.LerTexto("Cidade");
            var capacidade = Console2.LerInteiro("Capacidade máxima", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO Local (nomeLocal, endereco, cidade, capacidade) VALUES (@nome,@end,@cid,@cap)", conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@end", endereco);
                cmd.Parameters.AddWithValue("@cid", cidade);
                cmd.Parameters.AddWithValue("@cap", capacidade);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Local cadastrado com sucesso!");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        public static void ListarLocais()
        {
            Console2.Titulo("LISTA DE LOCAIS");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand("SELECT idLocal, nomeLocal, endereco, cidade, capacidade FROM Local ORDER BY nomeLocal", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Nome",-25} {"Endereço",-30} {"Cidade",-20} {"Cap.",-6}");
                Console2.Linha();

                bool tem = false;
                while (reader.Read())
                {
                    tem = true;
                    Console2.Info($"{reader["idLocal"],-5} {reader["nomeLocal"],-25} {reader["endereco"],-30} {reader["cidade"],-20} {reader["capacidade"],-6}");
                }
                if (!tem) Console2.Info("Nenhum local cadastrado.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }

        private static void ExcluirLocal()
        {
            Console2.Titulo("EXCLUIR LOCAL");
            ListarLocais();
            var id = Console2.LerInteiro("ID do local a excluir", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                Console.Write("\n  Confirma exclusão? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s") { Console2.Info("Cancelado."); Console2.PressEnter(); return; }

                var cmd = new MySqlCommand("DELETE FROM Local WHERE idLocal=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Local excluído com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                Console2.Erro("Não é possível excluir: local possui eventos vinculados.");
            }
            catch (Exception ex) { Console2.Erro($"Erro: {ex.Message}"); }
            Console2.PressEnter();
        }
    }
}
