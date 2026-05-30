using MySql.Data.MySqlClient;

namespace EventosApp
{
    public static class OrganizadorMenu
    {
        public static void Exibir()
        {
            while (true)
            {
                int op = Console2.Menu("ORGANIZADORES", new[]
                {
                    "Cadastrar organizador",
                    "Listar organizadores",
                    "Editar organizador",
                    "Excluir organizador"
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
            Console2.Titulo("CADASTRAR ORGANIZADOR");
            var nome = Console2.LerTexto("Nome");
            var email = Console2.LerTexto("E-mail");
            var telefone = Console2.LerTexto("Telefone", obrigatorio: false);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO Organizador (nome, email, telefone) VALUES (@nome, @email, @tel)", conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@tel", telefone == "" ? DBNull.Value : (object)telefone);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Organizador cadastrado com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console2.Erro("Este e-mail já está cadastrado.");
            }
            catch (Exception ex)
            {
                Console2.Erro($"Erro ao cadastrar: {ex.Message}");
            }
            Console2.PressEnter();
        }

        public static void Listar()
        {
            Console2.Titulo("LISTA DE ORGANIZADORES");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT idOrganizador, nome, email, telefone FROM Organizador ORDER BY nome", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Nome",-30} {"E-mail",-30} {"Telefone",-15}");
                Console2.Linha();

                bool temRegistros = false;
                while (reader.Read())
                {
                    temRegistros = true;
                    Console2.Info($"{reader["idOrganizador"],-5} {reader["nome"],-30} {reader["email"],-30} {(reader["telefone"] == DBNull.Value ? "-" : reader["telefone"]),-15}");
                }
                if (!temRegistros) Console2.Info("Nenhum organizador cadastrado.");
            }
            catch (Exception ex)
            {
                Console2.Erro($"Erro ao listar: {ex.Message}");
            }
            Console2.PressEnter();
        }

        private static void Editar()
        {
            Console2.Titulo("EDITAR ORGANIZADOR");
            Listar();
            var id = Console2.LerInteiro("ID do organizador a editar", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                var check = new MySqlCommand("SELECT COUNT(*) FROM Organizador WHERE idOrganizador=@id", conn);
                check.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    Console2.Erro("Organizador não encontrado.");
                    Console2.PressEnter();
                    return;
                }

                var nome = Console2.LerTexto("Novo nome");
                var email = Console2.LerTexto("Novo e-mail");
                var telefone = Console2.LerTexto("Novo telefone", obrigatorio: false);

                var cmd = new MySqlCommand(
                    "UPDATE Organizador SET nome=@nome, email=@email, telefone=@tel WHERE idOrganizador=@id", conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@tel", telefone == "" ? DBNull.Value : (object)telefone);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Organizador atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                Console2.Erro($"Erro ao editar: {ex.Message}");
            }
            Console2.PressEnter();
        }

        private static void Excluir()
        {
            Console2.Titulo("EXCLUIR ORGANIZADOR");
            Listar();
            var id = Console2.LerInteiro("ID do organizador a excluir", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                var check = new MySqlCommand("SELECT COUNT(*) FROM Organizador WHERE idOrganizador=@id", conn);
                check.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    Console2.Erro("Organizador não encontrado.");
                    Console2.PressEnter();
                    return;
                }

                Console.Write("\n  Confirma exclusão? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s") { Console2.Info("Exclusão cancelada."); Console2.PressEnter(); return; }

                var cmd = new MySqlCommand("DELETE FROM Organizador WHERE idOrganizador=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Organizador excluído com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                Console2.Erro("Não é possível excluir: este organizador possui eventos vinculados.");
            }
            catch (Exception ex)
            {
                Console2.Erro($"Erro ao excluir: {ex.Message}");
            }
            Console2.PressEnter();
        }
    }
}
