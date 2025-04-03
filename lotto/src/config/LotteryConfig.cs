namespace Lotto.Config;

public struct LotteryConfiguration
{
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int MinTickets { get; set; }
    public int MaxTickets { get; set; }
    public double TicketPrice { get; set; }

    public double HumanPlayerStartingBalance { get; set; }
    public bool HumanAllowPlayerCredit { get; set; }

    public double AIPlayerStartingBalance { get; set; }
    public bool AIAllowPlayerCredit { get; set; }

    public double GrandPrizeWinningsPercentage { get; set; }
    public double Tier2WinPercentage { get; set; }
    public double Tier2WinningsShare { get; set; }
    public double Tier3WinPercentage { get; set; }
    public double Tier3WinningsShare { get; set; }

    public bool AllowMultiPrize { get; set; }
    public string SplitResolution { get; set; }
}