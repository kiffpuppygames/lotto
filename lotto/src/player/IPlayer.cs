namespace Lotto.Player;

public interface IPlayer
{
    public uint Id { get; init; }
    public string Name { get; init; }
    public double Balance { get; set; }
    public uint TicketsPurchased { get; set; }
    public List<ulong> TicketIds { get; set; }
}