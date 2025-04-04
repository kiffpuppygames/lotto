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

public class CommandProcessor
{
    public static void DispatchCommand(ICommand command)
    {
        // Add the command to the dictionary
        Game.Instance.Commands.Enqueue(command);
    }

    public static CommandProccessorResult<CommandResultType> ProcessCommands()
    {
        while (Game.Instance.Commands.Count > 0)
        {
            var command = Game.Instance.Commands.Dequeue();
            
            switch (command)
            {
                case CreatePlayerCommand createPlayerCommand:
                    var res = PlayerService.ProcessCreatePlayerCommand(createPlayerCommand);

                    switch (res)
                    {
                        case MaxPlayersReached<IPlayer> maxPlayersReached:
                            Console.WriteLine($"Failed to create player. {createPlayerCommand} {maxPlayersReached}");
                            break;
                        case InvalidPlayerTypeResult<IPlayer> invalidPlayerType:
                            Console.WriteLine($"Failed to create player. {createPlayerCommand} {invalidPlayerType}");
                            break;
                    }
                    break;
                case PurchaseTicketsCommand purchaseTicketCommand:
                    res = PlayerService.ProcessPurchaseTicketsCommand(purchaseTicketCommand);
                    if (res is InsufficientFunds<IPlayer> purchase_res)
                        Console.WriteLine($"Failed to process purchase command. {purchaseTicketCommand} {purchase_res}");
                    break;
                case DrawResultsCommand drawCommand:
                    if (ResultsService.ProcessDrawResultsCommand(drawCommand) is not OkResult<DrawResult>)
                        Console.WriteLine("Draw Failed");
                    break;
                case DisplayResultsCommand displayCommand:
                    if (Game.Instance.LottoResult != null)
                        ResultsService.DisplayResults(Game.Instance.LottoResult);
                    else
                        Console.WriteLine("Results have not yet been calculated");
                    break;
                case ExitGameCommand exitGameCommand:
                    Game.Instance.IsRunning = false;
                    return new CommandProccessorResult<CommandResultType>(CommandResultType.Exited);                
                default:
                    return new CommandProccessorResult<CommandResultType>(CommandResultType.Error);
            }
        }

        return new CommandProccessorResult<CommandResultType>(CommandResultType.Exited);
    }
}