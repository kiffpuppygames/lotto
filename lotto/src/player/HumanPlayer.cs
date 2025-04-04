using System.Runtime.InteropServices;

namespace Lotto.Player;

public record HumanPlayer : IPlayer
{
    public uint Id { get; init; }
    public string Name { get; init; }
    public double Balance { get; set; }
    public uint TicketsPurchased { get; set; }
    public List<ulong> TicketIds { get; set; }

    public HumanPlayer(ulong id, string name, double balance)
    {
        Id = (uint)id;
        Name = $"Player 1: {name}";
        Balance = balance;
        TicketsPurchased = 0;
        TicketIds = [];
    }
}