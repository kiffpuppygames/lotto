namespace Lotto.Player;

public enum PlayerType
{
    Human,
    AI
}

public record MaxPlayersReached<T> : ErrorResult<T>;
public record InsufficientFunds<T> : ErrorResult<T>;

public record InvalidPlayerNameResult<T> : ErrorResult<T>;
public record InvalidPlayerTypeResult<T> : ErrorResult<T>;

public record InvalidPurchaseTicketsCommand<T> : ErrorResult<T>;

public record CreatePlayerCommand(in PlayerType PlayerType, in string? Name, uint TicketsToPurchase) : ICommand
{
    public ulong Id { get; init; } = Game.Instance.IdProvider.NextCommandId;
    public PlayerType PlayerType { get; init; } = PlayerType;
    public string? Name { get; init; } = Name;
    public uint TicketsPurchased { get; init; } = TicketsToPurchase;
}

public record PurchaseTicketsCommand(IPlayer Player, in uint NumberOfTickets) : ICommand
{
    public ulong Id { get; init; } = Game.Instance.IdProvider.NextCommandId;
    public IPlayer Player { get; init; } = Player;
    public uint NumberOfTickets { get; init; } = NumberOfTickets;
}

public class PlayerService
{
    public static Result<IPlayer> ProcessCreatePlayerCommand(CreatePlayerCommand command)
    {
        if (Game.Instance.Players.Count >= Game.Instance.Config.MaxPlayers) 
            return new MaxPlayersReached<IPlayer>();

        switch (command.PlayerType)
        {
            case PlayerType.Human:
                if (string.IsNullOrWhiteSpace(command.Name)) 
                    return new InvalidPlayerNameResult<IPlayer>();

                var human = new HumanPlayer(Game.Instance.IdProvider.NextPlayerId, command.Name, Game.Instance.Config.HumanPlayerStartingBalance);
                Game.Instance.Players.Add(human);

                CommandProcessor.DispatchCommand(new PurchaseTicketsCommand(
                    human, 
                    command.TicketsToPurchase)
                );
                break;
            case PlayerType.AI: 
                var ai = new AIPlayer(Game.Instance.IdProvider.NextPlayerId, Game.Instance.Config.HumanPlayerStartingBalance);
                Game.Instance.Players.Add(ai);
                
                CommandProcessor.DispatchCommand(new PurchaseTicketsCommand(
                    ai, 
                    command.TicketsToPurchase)
                );
                break;
            default:
                return new InvalidPlayerTypeResult<IPlayer>();
        }
        
        return new OkResult<IPlayer>(Game.Instance.Players.Last());
    }
    
    public static Result<IPlayer> ProcessPurchaseTicketsCommand(PurchaseTicketsCommand command)
    {
        if (command.NumberOfTickets <= 0) return new InvalidPurchaseTicketsCommand<IPlayer>();

        double totalCost = command.NumberOfTickets * Game.Instance.Config.TicketPrice;
        uint maxAffordableTickets = (uint)(command.Player.Balance / Game.Instance.Config.TicketPrice);
        uint ticketsToPurchase = command.NumberOfTickets > maxAffordableTickets ? maxAffordableTickets : command.NumberOfTickets;

        var tickets = Enumerable.Range(0, (int)ticketsToPurchase)
            .Select(_ => new Ticket(Game.Instance.IdProvider.NextTicketId, command.Player.Id))
            .ToList();
            
        Game.Instance.Tickets.AddRange(tickets);
        
        command.Player.TicketIds.AddRange(tickets.Select(ticket => ticket.Id));

        command.Player.Balance -= maxAffordableTickets * Game.Instance.Config.TicketPrice;
        command.Player.TicketsPurchased += command.NumberOfTickets;
        
        return new OkResult<IPlayer>(command.Player);
    }

    
}