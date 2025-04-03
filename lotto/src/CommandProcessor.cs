using System.Diagnostics;
using Lotto.Player;

namespace Lotto;

public record UnhandledCommandType<T> : ErrorResult<T>;

public record CommandProccessorResult<CommandResultType> : Result<CommandResultType>
{
    public CommandResultType ResultType { get; init; }

    public CommandProccessorResult(CommandResultType resultType)
    {
        ResultType = resultType;
    }
}

public enum CommandResultType
{
    Ok,
    Error,
    Exited,
}

public class CommandProcessor(in Game game)
{
    private Game _game = game;

    public void DispatchCommand(ICommand command)
    {
        // Add the command to the dictionary
        _game.Commands.Enqueue(command);
    }

    public CommandProccessorResult<CommandResultType> ProcessCommands()
    {
        while (_game.Commands.Count > 0)
        {
            var command = _game.Commands.Dequeue();
            
            switch (command)
            {
                case CreatePlayerCommand createPlayerCommand:
                    var res = PlayerService.ProcessCreatePlayerCommand(createPlayerCommand);
                    if (res is InvalidPlayerNameResult<IPlayer> create_res)
                        Debug.Print($"Failed to process create player command. {createPlayerCommand} {create_res}");

                    break;
                case PurchaseTicketsCommand purchaseTicketCommand:
                    res = PlayerService.ProcessPurchaseTicketsCommand(purchaseTicketCommand);
                    if (res is InsufficientFunds<IPlayer> purchase_res)
                        Debug.Print($"Failed to process purchase command. {purchaseTicketCommand} {purchase_res}");
                    break;
                case DrawResultsCommand drawCommand:
                    if (ResultsService.ProcessDrawResultsCommand(drawCommand) is OkResult<DrawResult> drawResult)
                        Console.WriteLine("Draw Complete");
                    break;
                case DisplayResultsCommand displayCommand:
                    if (Game.Instance.LottoResult != null)
                        ResultsService.DisplayResults(Game.Instance.LottoResult);
                    else
                        Console.WriteLine("Result have not yet been calculated");
                    break;
                case ExitGameCommand exitGameCommand:
                    _game.IsRunning = false;
                    return new CommandProccessorResult<CommandResultType>(CommandResultType.Exited);
                default:
                    return new CommandProccessorResult<CommandResultType>(CommandResultType.Error);
            }
        }

        return new CommandProccessorResult<CommandResultType>(CommandResultType.Exited);
    }
}