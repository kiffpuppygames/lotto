using Lotto;

namespace LottoTests;

public class ResultsTests : IDisposable
{
    static readonly int SEED = 1234567890;

    [Fact]
    public void ValidateDrawResults()
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
                Game.Instance.CommandProcessor.DispatchCommand(new Lotto.Player.CreatePlayerCommand(Lotto.Player.PlayerType.AI, null, ticketsToPurchase));
            }

            if (Game.Instance.CommandProcessor.ProcessCommands() is CommandProccessorResult<CommandResultType> procResult)
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

            Game.Instance.CommandProcessor.DispatchCommand(new Lotto.DrawResultsCommand());

            if (Game.Instance.CommandProcessor.ProcessCommands() is CommandProccessorResult<CommandResultType> drawResult)
            {
                Assert.True(drawResult.ResultType == CommandResultType.Exited, $"Expected Ok, but got {drawResult.ResultType}.");
            }
            else
            {
                Assert.Fail("Failed to process draw results command.");
            }
            
            var lottoResult = Game.Instance.LottoResult;
            Assert.NotNull(lottoResult);

            var allWinningTickets = new List<ulong>
            {
                lottoResult.GrandPrizeWinner.Id
            };

            allWinningTickets.AddRange(lottoResult.Tier2Winners.Select(ticket => ticket.Id));
            allWinningTickets.AddRange(lottoResult.Tier3Winners.Select(ticket => ticket.Id));

            var duplicateTickets = allWinningTickets
                .GroupBy(ticketId => ticketId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            Assert.True(duplicateTickets.Count == 0, $"Duplicate winning tickets found: {string.Join(", ", duplicateTickets)}");
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
    }
}
