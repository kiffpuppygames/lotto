using System.Threading.Tasks;
using System.Diagnostics;

namespace Lotto;

public class Program
{
    public static void Main(string[] args)
    {    
        StartGame();

        Console.WriteLine("Starting game. 'Esc' to exit.");
        while (Game.Instance.IsRunning)
        {
            var key = Console.ReadKey(intercept: true);

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    Game.Instance.CommandProcessor.DispatchCommand(new ExitGameCommand());
                    break;
                case ConsoleKey.F5:
                    Game.Instance.CommandProcessor.DispatchCommand(new DrawResultsCommand());
                    Game.Instance.CommandProcessor.DispatchCommand(new DisplayResultsCommand());
                    break;
                default:
                    Console.WriteLine("Unhandled Input");
                    break;
            }
        }

        Game.Deinit();
        Console.WriteLine("Game exited.");
    }

    public static void StartGame() 
    {
        Game.Init((int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 5);
        
        var numberOfPlayers = Game.Instance.Random.Next(Game.Instance.Config.MinPlayers, Game.Instance.Config.MaxPlayers);

        Console.WriteLine("\nEnter player name:");
        string? playerName = Console.ReadLine();

        Console.WriteLine("\nHow many Tickets do you wish to purchase?");
        string? ticketsToPurchaseInput = Console.ReadLine();

        if (uint.TryParse(ticketsToPurchaseInput, out uint ticketsToPurchase) && ticketsToPurchase >= Game.Instance.Config.MinTickets)
        {
            Game.Instance.CommandProcessor.DispatchCommand(new Player.CreatePlayerCommand(Player.PlayerType.Human, playerName, ticketsToPurchase));
        }

        for (int i = 0; i < numberOfPlayers - 1; i++)
        {
            Game.Instance.CommandProcessor.DispatchCommand(new Lotto.Player.CreatePlayerCommand(Lotto.Player.PlayerType.AI, null, (uint)Game.Instance.Random.Next(Game.Instance.Config.MinTickets, Game.Instance.Config.MaxTickets)));
        }        

        Task.Run(() => {
            while (Game.Instance.IsRunning)
            {
                Game.Instance.CommandProcessor.ProcessCommands();
            }
        });
    }
}