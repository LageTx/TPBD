using EventosApp;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Title = "Sistema de Gerenciamento de Eventos";

// Testa conexão ao iniciar
Console.WriteLine("\nConectando ao banco de dados...");

if (!Database.TestarConexao())
{
    Console.WriteLine("Falha ao conectar ao banco de dados. Verifique as configuracoes.");
    Console.ReadLine();
    return;
}

Console.WriteLine("Conexao estabelecida com sucesso!");
System.Threading.Thread.Sleep(1000);

// Menu principal
while (true)
{
    int op = Console2.Menu("SISTEMA DE GERENCIAMENTO DE EVENTOS", new[]
    {
        "Organizadores",
        "Participantes",
        "Categorias e Locais",
        "Eventos",
        "Venda de Ingressos",
        "Pagamentos",
        "Relatórios e Consultas"
    });

    switch (op)
    {
        case 1: OrganizadorMenu.Exibir(); break;
        case 2: ParticipanteMenu.Exibir(); break;
        case 3: CategoriaLocalMenu.Exibir(); break;
        case 4: EventoMenu.Exibir(); break;
        case 5: IngressoMenu.Exibir(); break;
        case 6: PagamentoMenu.Exibir(); break;
        case 7: RelatorioMenu.Exibir(); break;
        case 0:
            Console2.Titulo("SAINDO DO SISTEMA");
            Console2.Info("Até logo!");
            Console.WriteLine();
            return;
    }
}
