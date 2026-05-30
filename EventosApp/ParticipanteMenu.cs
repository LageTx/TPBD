using MySql.Data.MySqlClient;

namespace EventosApp
{
    public static class ParticipanteMenu
    {
        public static void Exibir()
        {
            while (true)
            {
                int op = Console2.Menu("PARTICIPANTES", new[]
                {
                    "Cadastrar participante",
                    "Listar participantes",
                    "Editar participante",
                    "Excluir participante"
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
            Console2.Titulo("CADASTRAR PARTICIPANTE");
            var nome = Console2.LerTexto("Nome");
            var cpf = Console2.LerTexto("CPF (somente números)");
            var email = Console2.LerTexto("E-mail");
            var telefone = Console2.LerTexto("Telefone", obrigatorio: false);
            var dataNasc = Console2.LerData("Data de nascimento");

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    "INSERT INTO Participante (nome, cpf, email, telefone, dataNascimento) VALUES (@nome,@cpf,@email,@tel,@nasc)", conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@cpf", cpf);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@tel", telefone == "" ? DBNull.Value : (object)telefone);
                cmd.Parameters.AddWithValue("@nasc", dataNasc.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Participante cadastrado com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console2.Erro("CPF ou e-mail já cadastrado.");
            }
            catch (Exception ex)
            {
                Console2.Erro($"Erro ao cadastrar: {ex.Message}");
            }
            Console2.PressEnter();
        }

        public static void Listar()
        {
            Console2.Titulo("LISTA DE PARTICIPANTES");
            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT idParticipante, nome, cpf, email, telefone FROM Participante ORDER BY nome", conn);
                using var reader = cmd.ExecuteReader();

                Console.WriteLine($"\n  {"ID",-5} {"Nome",-28} {"CPF",-13} {"E-mail",-28} {"Telefone",-15}");
                Console2.Linha();

                bool temRegistros = false;
                while (reader.Read())
                {
                    temRegistros = true;
                    Console2.Info($"{reader["idParticipante"],-5} {reader["nome"],-28} {reader["cpf"],-13} {reader["email"],-28} {(reader["telefone"] == DBNull.Value ? "-" : reader["telefone"]),-15}");
                }
                if (!temRegistros) Console2.Info("Nenhum participante cadastrado.");
            }
            catch (Exception ex)
            {
                Console2.Erro($"Erro ao listar: {ex.Message}");
            }
            Console2.PressEnter();
        }

        private static void Editar()
        {
            Console2.Titulo("EDITAR PARTICIPANTE");
            Listar();
            var id = Console2.LerInteiro("ID do participante a editar", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                var check = new MySqlCommand("SELECT COUNT(*) FROM Participante WHERE idParticipante=@id", conn);
                check.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    Console2.Erro("Participante não encontrado.");
                    Console2.PressEnter();
                    return;
                }

                var nome = Console2.LerTexto("Novo nome");
                var email = Console2.LerTexto("Novo e-mail");
                var telefone = Console2.LerTexto("Novo telefone", obrigatorio: false);

                var cmd = new MySqlCommand(
                    "UPDATE Participante SET nome=@nome, email=@email, telefone=@tel WHERE idParticipante=@id", conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@tel", telefone == "" ? DBNull.Value : (object)telefone);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Participante atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                Console2.Erro($"Erro ao editar: {ex.Message}");
            }
            Console2.PressEnter();
        }

        private static void Excluir()
        {
            Console2.Titulo("EXCLUIR PARTICIPANTE");
            Listar();
            var id = Console2.LerInteiro("ID do participante a excluir", 1);

            try
            {
                using var conn = Database.ObterConexao();
                conn.Open();

                var check = new MySqlCommand("SELECT COUNT(*) FROM Participante WHERE idParticipante=@id", conn);
                check.Parameters.AddWithValue("@id", id);
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    Console2.Erro("Participante não encontrado.");
                    Console2.PressEnter();
                    return;
                }

                Console.Write("\n  Confirma exclusão? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s") { Console2.Info("Exclusão cancelada."); Console2.PressEnter(); return; }

                var cmd = new MySqlCommand("DELETE FROM Participante WHERE idParticipante=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Console2.Sucesso("Participante excluído com sucesso!");
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                Console2.Erro("Não é possível excluir: participante possui ingressos vinculados.");
            }
            catch (Exception ex)
            {
                Console2.Erro($"Erro ao excluir: {ex.Message}");
            }
            Console2.PressEnter();
        }
    }
}
