using System.Dynamic;

namespace Lotto;

public record DisplayResultsCommand : ICommand
{
    public ulong Id { get; init; }
}

public record DrawResult(
    double TotalPot,
    Ticket GrandPrizeWinner, double GrandPrizePot,
    List<Ticket> Tier2Winners, double Tier2PrizePot, 
    List<Ticket> Tier3Winners, double Tier3PrizePot)
{
    public double TotalPot { get; } = TotalPot;

    public Ticket GrandPrizeWinner { get; } = GrandPrizeWinner;
    public double GrandPrizePot { get; } = GrandPrizePot;

    public List<Ticket> Tier2Winners { get; } = Tier2Winners;
    public double Tier2PrizePot { get; } = Tier2PrizePot;

    public List<Ticket> Tier3Winners { get; } = Tier3Winners;
    public double Tier3PrizePot { get; } = Tier3PrizePot;
}

public class ResultsService
{
    public static Result<DrawResult> ProcessDrawResultsCommand(DrawResultsCommand command)
    {
        var tickets = Game.Instance.Tickets.ToList();
        var totalPot = Game.Instance.Tickets.Count * Game.Instance.Config.TicketPrice;

        var grandPrizeWinningTicket = tickets[Game.Instance.Random.Next(tickets.Count)];
        var grandPrizePot = Game.Instance.Config.GrandPrizeWinningsPercentage/100 * totalPot;

        tickets.Remove(grandPrizeWinningTicket);
        
        var tier2Count = Game.Instance.Config.SplitResolution switch
        {
            "round_up" => (int)Math.Ceiling(Game.Instance.Config.Tier2WinPercentage / 100 * tickets.Count),
            "round_down" => (int)Math.Floor(Game.Instance.Config.Tier2WinPercentage / 100 * tickets.Count),
            _ => (int)Math.Round(Game.Instance.Config.Tier2WinPercentage / 100 * tickets.Count),
        };
        
        var tier2Winners = tickets
            .OrderBy(_ => Game.Instance.Random.Next())
            .Take(tier2Count)
            .ToList();

        tickets.RemoveAll(tier2ticket => tier2Winners.Contains(tier2ticket));

        var tier2PrizePot = Game.Instance.Config.Tier2WinningsShare / 100 * totalPot;

        var tier3Count = Game.Instance.Config.SplitResolution switch
        {
            "round_up" => (int)Math.Ceiling(Game.Instance.Config.Tier3WinPercentage / 100 * tickets.Count),
            "round_down" => (int)Math.Floor(Game.Instance.Config.Tier3WinPercentage / 100 * tickets.Count),
            _ => (int)Math.Round(Game.Instance.Config.Tier3WinPercentage / 100 * tickets.Count),
        };

        var tier3Winners = tickets
            .OrderBy(_ => Game.Instance.Random.Next())
            .Take(tier3Count)
            .ToList();

        tickets.RemoveAll(tier3ticket => tier3Winners.Contains(tier3ticket));
            
        var tier3PrizePot = Game.Instance.Config.Tier3WinningsShare / 100 * totalPot;

        var drawResult = new DrawResult(
            totalPot, 
            grandPrizeWinningTicket, grandPrizePot, 
            tier2Winners, tier2PrizePot, 
            tier3Winners, tier3PrizePot);

        Game.Instance.LottoResult = drawResult;

        return new OkResult<DrawResult>(drawResult);
    }

    public static void DisplayResults(DrawResult drawResult)
    {
        Console.WriteLine($"Total sales: ${drawResult.TotalPot}");

        Console.WriteLine($"Grand prize winner, {Game.Instance.Players[(int)drawResult.GrandPrizeWinner.Owner].Name}, wins ${drawResult.GrandPrizePot} with ticket {drawResult.GrandPrizeWinner.Id}");
        
        Console.WriteLine("Tier 2 Winners:");
        foreach(var winner in drawResult.Tier2Winners)
        {
            Console.WriteLine($"\t {Game.Instance.Players[(int)winner.Owner-1].Name} with ticket {winner.Id}");
        }
        Console.WriteLine($"Each win ${drawResult.Tier2PrizePot / drawResult.Tier2Winners.Count} of ${drawResult.Tier2PrizePot}");

        Console.WriteLine("Tier 3 Winners:");
        foreach(var winner in drawResult.Tier3Winners)
        {
            Console.WriteLine($"\t {Game.Instance.Players[(int)winner.Owner - 1].Name} with ticket {winner.Id}");
        }
        Console.WriteLine($"Each win ${drawResult.Tier3PrizePot / drawResult.Tier3Winners.Count} of ${drawResult.Tier3PrizePot}");

        Console.WriteLine($"The House Takes ${drawResult.TotalPot - drawResult.GrandPrizePot - drawResult.Tier2PrizePot - drawResult.Tier3PrizePot}");
    }
}