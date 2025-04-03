namespace Lotto.Player;

public record AIPlayer : IPlayer
{
    public uint Id { get; init; }
    public string Name { get; init; }
    public double Balance { get; set; }
    public uint TicketsPurchased { get; set; }
    public List<ulong> TicketIds { get; set; }

    private static readonly string NAME_PREFIX = "Player ";

    public AIPlayer(ulong id, double balance)
    {
        Id = (uint)id;
        Name = $"{NAME_PREFIX}{id}";
        Balance = balance;
        TicketsPurchased = 0;
        TicketIds = [];
    }
}