public record Ticket(ulong id, ulong owner)
{
    private readonly ulong _id = id;
    private readonly ulong _ownerId = owner;
    public ulong Id => _id;
    public ulong Owner => _ownerId;    
}