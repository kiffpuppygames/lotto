using Lotto;

namespace LottoTests;

public class PlayerTests : IDisposable
{
    static readonly int SEED = 1234567890;

    [Fact]
    public void AttemptToPurchaseMoreTicketsThanPossible()
    { 
        if (Game.IsInitilized())
            Game.Deinit();
        
        Game.Init(SEED); 

        try
        {
            CommandProcessor.DispatchCommand(new Lotto.Player.CreatePlayerCommand(Lotto.Player.PlayerType.AI, null, 20));
            var processResult = CommandProcessor.ProcessCommands();
            
            if (processResult is CommandProccessorResult<CommandResultType> result)
            {
                Assert.True(result.ResultType == CommandResultType.Exited , $"Expected Ok, but got {result.ResultType}.");
            }
            else
            {
                Assert.Fail("Failed to process command.");
            }

            Assert.True(Game.Instance.Players[0].Balance == 0, $"Expected balance to be 0, but found {Game.Instance.Players[0].Balance}.");
            Assert.True(Game.Instance.Players[0].TicketIds.Count == 10, $"Expected purchased tickets to be 10, but found {Game.Instance.Players[0].TicketIds.Count}.");
        }
        finally
        {
            Game.Deinit();
        }
    }

    [Fact]
    public void CreateAIPlayers()
    {
        if (Game.IsInitilized())
            Game.Deinit();
        
        Game.Init(SEED); 

        try
        {
            CommandProcessor.DispatchCommand(new Lotto.Player.CreatePlayerCommand(Lotto.Player.PlayerType.AI, null, 0));
            var processResult = CommandProcessor.ProcessCommands();

            if (processResult is CommandProccessorResult<CommandResultType> result)
            {
                Assert.True(result.ResultType == CommandResultType.Exited , $"Expected Ok, but got {result.ResultType}.");
            }
            else
            {
                Assert.Fail("Failed to process command.");
            }

            Assert.True(Game.Instance.Players.Count == 1, $"Expected 1 player, but got {Game.Instance.Players.Count}.");
        }
        finally
        {
            Game.Deinit();
        }
    }

    [Fact]
    public void MultiplePlayersPurchaseMultipleTickets()
    {
        if (Game.IsInitilized())
            Game.Deinit();
        
        Game.Init(SEED); 

        try
        {
            uint ticketsToPurchase = 3;
            var numberOfPlayers = Game.Instance.Random.Next(Game.Instance.Config.MinPlayers, Game.Instance.Config.MaxPlayers + 1);
            
            for (int i = 0; i < numberOfPlayers; i++)
            {
                CommandProcessor.DispatchCommand(new Lotto.Player.CreatePlayerCommand(Lotto.Player.PlayerType.AI, null, ticketsToPurchase));
            }

            if (CommandProcessor.ProcessCommands() is CommandProccessorResult<CommandResultType> procResult)
            {
                Assert.True(procResult.ResultType == CommandResultType.Exited , $"Expected Ok, but got {procResult.ResultType}.");
            }
            else
            {
                Assert.Fail("Failed to process create commands.");
            }

            Assert.True(Game.Instance.Players.Count == numberOfPlayers, $"Expected {numberOfPlayers} players, but got {Game.Instance.Players.Count}.");

            var totalPlayerTickets = 0;
            foreach (var player in Game.Instance.PlayersReadOnly)
            {
                Assert.True(player.TicketIds.Count == ticketsToPurchase, $"Expected {ticketsToPurchase} ticket id, but got {player.TicketIds.Count}.");  
                totalPlayerTickets += player.TicketIds.Count;
            }

            Assert.True(Game.Instance.Tickets.Count == totalPlayerTickets, $"Expected {totalPlayerTickets} ticket, but got {Game.Instance.Tickets.Count}.");
        }
        finally
        {
            Game.Deinit();
        }
    }

    public void Dispose()
    {
        if (Game.IsInitilized())
            Game.Deinit();

        GC.SuppressFinalize(this);
    }
}