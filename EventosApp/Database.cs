using MySql.Data.MySqlClient;

namespace EventosApp
{
    public static class Database
    {
        private const string ConnectionString =
            "Server=caboose.proxy.rlwy.net;" +
            "Port=22369;" +
            "Database=railway;" +
            "Uid=root;" +
            "Pwd=jtLxaACceQLSCnboEjPDsAkkjjZCGvTK;" +
            "SslMode=none;";

        public static MySqlConnection ObterConexao()
        {
            return new MySqlConnection(ConnectionString);
        }

        public static bool TestarConexao()
        {
            try
            {
                using var conn = ObterConexao();
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
