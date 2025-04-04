using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using Lotto.Config;
using Lotto.Player;

namespace Lotto;

public record GameInitResult<CommandResultType>() : Result<CommandResultType>;

public enum GameInitResultType
{
    Ok,
    Error,
}

public record DrawResultsCommand : ICommand
{
    public ulong Id { get; init; } = Game.Instance.IdProvider.NextCommandId;
}

public record ExitGameCommand : ICommand
{
    public ulong Id { get; init; } = Game.Instance.IdProvider.NextCommandId;
}

public class IdProvider
{
    private ulong _nextPlayerId = 0;
    public ulong NextPlayerId => Interlocked.Increment(ref _nextPlayerId);

    private ulong _nextCommnadId = 0;
    public ulong NextCommandId => Interlocked.Increment(ref _nextCommnadId);

    private ulong _nextTicketId = 0;
    public ulong NextTicketId => Interlocked.Increment(ref _nextTicketId);

    public void ResetPlayerIds() => Interlocked.Exchange(ref _nextPlayerId, 0);
    public void ResetCommnadIds() => Interlocked.Exchange(ref _nextCommnadId, 0);
    public void ResetTicketIds() => Interlocked.Exchange(ref _nextTicketId, 0);
}

public record Game
{
    private static Game? _instance;
    private static readonly object _lock = new();
    public static Game Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    Console.WriteLine("Game instance has not been initialized. Call Initialize first.");
                    return null!;
                }
                return _instance;
            }
        }
    }

    public static void Init(int ?randomSeed = null)
    {
        lock (_lock)
        {
            if (_instance != null)
            {
                Console.WriteLine("Game instance has already been initialized.");
                return;
            }
            _instance = new Game(randomSeed);
        }
    }

    public static void Deinit()
    {
        lock (_lock)
        {            
            if (_instance != null)
            {
                _instance.Players.Clear();
                _instance.Tickets.Clear();
                _instance = null;
            }
        }
    }

    public static bool IsInitilized()
    {
        lock (_lock)
        {  
            if (_instance == null)
                return false;

            return true;
        }
    }

    private Game(int ?randomSeed = null)
    {
        var configLoadRes = Lotto.Config.ConfigService.LoadAndValidateLotteryConfig();

        if (configLoadRes is OkResult<LotteryConfiguration> config)
        {
            Config = config.Value;

            if (randomSeed != null)
            {
                Random = new Random(randomSeed.Value);
                Debug.Print("Game Initilized with Seed");                
            }
            else
                Random = new Random();

            IdProvider = new();
            CommandProcessor = new CommandProcessor();
            _players = [];
        }
        else
        {
            Console.WriteLine("Config load failed");
            Environment.Exit(1);
        }
    }

    ~Game()
    {
        _instance = null;
    }

    public bool IsRunning { get; set; } = true;

    public readonly LotteryConfiguration Config;   

    public Random Random;

    public IdProvider IdProvider { get; init; }
    
    public CommandProcessor CommandProcessor { get; init; }

    private List<IPlayer> _players;    

    public Queue<ICommand> Commands = new();    

    public ReadOnlyCollection<IPlayer> PlayersReadOnly => _players.AsReadOnly();
    public List<IPlayer> Players => _players;

    public List<Ticket> Tickets = [];

    public DrawResult ?LottoResult = null; 
}