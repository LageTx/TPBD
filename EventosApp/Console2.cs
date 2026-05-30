namespace EventosApp
{
    public static class Console2
    {
        public static void Titulo(string texto)
        {
            Console.WriteLine();
            Console.WriteLine($"=== {texto} ===");
        }

        public static void Subtitulo(string texto)
        {
            Console.WriteLine($"\n{texto}:");
        }

        public static void Sucesso(string texto)
        {
            Console.WriteLine($"\nSucesso: {texto}");
        }

        public static void Erro(string texto)
        {
            Console.WriteLine($"\nErro: {texto}");
        }

        public static void Info(string texto)
        {
            Console.WriteLine(texto);
        }

        public static string LerTexto(string label, bool obrigatorio = true)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var valor = Console.ReadLine()?.Trim() ?? "";
                if (!obrigatorio || valor.Length > 0)
                    return valor;
                Erro("Campo obrigatorio. Tente novamente.");
            }
        }

        public static int LerInteiro(string label, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var input = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(input, out int valor) && valor >= min && valor <= max)
                    return valor;
                Erro($"Digite um numero inteiro valido entre {min} e {max}.");
            }
        }

        public static decimal LerDecimal(string label, decimal min = 0)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var input = Console.ReadLine()?.Trim()?.Replace(",", ".") ?? "";
                if (decimal.TryParse(input, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal valor) && valor >= min)
                    return valor;
                Erro($"Digite um valor decimal valido (minimo {min:F2}).");
            }
        }

        public static DateTime LerData(string label)
        {
            while (true)
            {
                Console.Write($"{label} (dd/MM/yyyy): ");
                var input = Console.ReadLine()?.Trim() ?? "";
                if (DateTime.TryParseExact(input, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime data))
                    return data;
                Erro("Data invalida. Use o formato dd/MM/yyyy.");
            }
        }

        public static TimeSpan LerHora(string label)
        {
            while (true)
            {
                Console.Write($"{label} (HH:mm): ");
                var input = Console.ReadLine()?.Trim() ?? "";
                if (TimeSpan.TryParseExact(input, "hh\\:mm",
                    System.Globalization.CultureInfo.InvariantCulture, out TimeSpan hora))
                    return hora;
                Erro("Hora invalida. Use o formato HH:mm.");
            }
        }

        public static int Menu(string titulo, string[] opcoes)
        {
            Titulo(titulo);
            for (int i = 0; i < opcoes.Length; i++)
                Console.WriteLine($"[{i + 1}] {opcoes[i]}");
            Console.WriteLine("[0] Voltar");
            return LerInteiro("\nEscolha uma opcao", 0, opcoes.Length);
        }

        public static void PressEnter()
        {
            Console.Write("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }

        public static void Linha()
        {
            Console.WriteLine(new string('-', 50));
        }
    }
}
