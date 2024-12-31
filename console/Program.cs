using Gambling.Library;
using Gambling.Library.Games;
using Spectre.Console;

AnsiConsole.MarkupLine("[blue]Welcome Gambler![/]");

var randomness = new ProvablyFairRandomness(
    serverSeed: "ZME1CHY7DD",
    clientSeed: "f427ededbc8e385d285ffee7a61930721e6c044d46b6445c1b7e74575d06caac",
    initialNonce: 229
);

IBettingStrategy bettingStrategy = new ModifiedMartingale();
decimal initialBalance = 1000m;

Gambler gambler = new Gambler(bettingStrategy, initialBalance);

var game = new Plinko(randomness, rows: 8, risk: Risk.Low);

Setup setup = new Setup(game, gambler);
Stats stats = setup.Run();

var table = new Table();
table.Border = TableBorder.Square;
table.Expand();

// Styled headers
table.AddColumn(new TableColumn("[yellow]ID[/]").Centered());
table.AddColumn(new TableColumn("[yellow]Strategy[/]").Centered());
table.AddColumn(new TableColumn("[yellow]Before[/]").Centered());
table.AddColumn(new TableColumn("[yellow]After[/]").Centered());
table.AddColumn(new TableColumn("[yellow]Amount[/]").RightAligned());
table.AddColumn(new TableColumn("[yellow]Placed At[/]").Centered());
table.AddColumn(new TableColumn("[yellow]Result[/]").Centered());
table.AddColumn(new TableColumn("[yellow]Payout[/]").RightAligned());
table.AddColumn(new TableColumn("[yellow]Raw Result[/]").RightAligned());

foreach (var bet in stats.Bets)
{
    // Color code based on bet result
    string resultColor = bet.BetResult switch
    {
        BetResult.Won => "[green]",
        BetResult.Lost => "[red]",
        _ => "[grey]"
    };

    table.AddRow(
        bet.Id.ToString(),
        bet.Strategy.ToString(),
        $"{bet.BalanceBeforeBet:F2}",
        $"{bet.BalanceAfterBet:F2}",
        $"{bet.Amount:F2}",
        bet.PlacedAt.ToString("g"),
        $"{resultColor}{bet.BetResult}[/]",
        bet.Payout.HasValue ? $"{resultColor}${bet.Payout:F2}[/]" : "[grey]Pending[/]",
        bet.RawBetResult.HasValue ? $"{resultColor}{bet.RawBetResult:F2}[/]" : "[grey]Pending[/]"
    );
}

// Add a title
AnsiConsole.Write(new Rule("[white]Betting Statistics[/]"));
AnsiConsole.Write(table);

AnsiConsole.MarkupLine("[blue]Goodbye Gambler![/]");
AnsiConsole.MarkupLine("[underline red]The house always wins![/]");

