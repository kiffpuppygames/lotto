using System.Threading.Tasks;
using System.Diagnostics;
using Lotto.Player;

namespace Lotto;

public class Program
{
    public static void Main(string[] args)
    {    
        StartGame();

        while (Game.Instance.IsRunning)
        {
            var key = Console.ReadKey(intercept: true);

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    CommandProcessor.DispatchCommand(new ExitGameCommand());
                    break;
                case ConsoleKey.F5:
                    CommandProcessor.DispatchCommand(new DrawResultsCommand());
                    CommandProcessor.DispatchCommand(new DisplayResultsCommand());
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

        Console.WriteLine("\nPlayer 1, please enter your name:");
        string? playerName = Console.ReadLine();

        CommandProcessor.DispatchCommand(new Player.CreatePlayerCommand(Player.PlayerType.Human, playerName, 0)); 

        for (int i = 0; i < numberOfPlayers - 1; i++)
        {
            CommandProcessor.DispatchCommand(new Lotto.Player.CreatePlayerCommand(Lotto.Player.PlayerType.AI, null, (uint)Game.Instance.Random.Next(Game.Instance.Config.MinTickets, Game.Instance.Config.MaxTickets)));            
        }  
        Console.WriteLine($"{numberOfPlayers} AI players have been created, and purchased their tickets");     

        CommandProcessor.ProcessCommands();

        SetupPlayer();

        Task.Run(() => {
            while (Game.Instance.IsRunning)
            {
                CommandProcessor.ProcessCommands();
            }
        });
    }

    public static void SetupPlayer()
    {        
        Console.WriteLine($"Welcome to the Bede Lottery, {Game.Instance.Players[0].Name}.");
        Console.WriteLine($"Your digital balance: ${Game.Instance.Players[0].Balance:C}");
        Console.WriteLine($"Each Ticket Costs: ${Game.Instance.Config.TicketPrice:C}");
        
        Console.WriteLine("\nHow many Tickets do you wish to purchase?");
        string? ticketsToPurchaseInput = Console.ReadLine();
        uint loopLimit = 5;
        uint loopCounter = 0;
        while (loopCounter < loopLimit)
        {
            if (string.IsNullOrWhiteSpace(ticketsToPurchaseInput))
            {
                Console.WriteLine("Please enter a number greater than 0.");
                ticketsToPurchaseInput = Console.ReadLine();
            }

            if (uint.TryParse(ticketsToPurchaseInput, out uint ticketsToPurchase))
            {
                if (ticketsToPurchase > 0)
                {
                    CommandProcessor.DispatchCommand(new PurchaseTicketsCommand(Game.Instance.Players[0], ticketsToPurchase));
                    break;                    
                }
                else
                {
                    Console.WriteLine("Please enter a number greater than 0.");
                    ticketsToPurchaseInput = Console.ReadLine();
                }
            }
            
            loopCounter++;
            if (loopCounter > loopLimit)
            {
                Console.WriteLine("Too many invalid attempts. Exiting.");
            }
        }
        Console.WriteLine($"\nReady to Draw! ['Esc' to exit.] ['F5' to draw results.]");
    }
}