using Lotto;
using Lotto.Config;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace LottoTests;

public class ConfigTests
{
    public const string CONFIG_FILE_PATH = "config.json";
    public const string CONFIG_SCHEMA_PATH = "config_schema.json";

    public const string CONFIG_FILE_PATH_ERROR = "Config file does not exist.";
    public const string CONFIG_SCHEMA_PATH_ERROR = "Schema file does not exist.";

    [Fact]
    public void LoadAndValidateLotteryConfig()
    {        
        var loadRes = Lotto.Config.ConfigService.LoadAndValidateLotteryConfig();

        if (loadRes is OkResult<LotteryConfiguration> ok)
        {
            Assert.Equal(10, ok.Value.MinPlayers);
            Assert.Equal(15, ok.Value.MaxPlayers);
            Assert.Equal(1, ok.Value.MinTickets);
            Assert.Equal(10, ok.Value.MaxTickets);
            Assert.Equal(1, ok.Value.TicketPrice);

            Assert.Equal(10.0, ok.Value.HumanPlayerStartingBalance);
            Assert.True(ok.Value.HumanAllowPlayerCredit);

            Assert.Equal(10.0, ok.Value.AIPlayerStartingBalance);
            Assert.False(ok.Value.AIAllowPlayerCredit);

            Assert.Equal(50.0, ok.Value.GrandPrizeWinningsPercentage);
            Assert.Equal(10.0, ok.Value.Tier2WinPercentage);
            Assert.Equal(30.0, ok.Value.Tier2WinningsShare);
            Assert.Equal(20.0, ok.Value.Tier3WinPercentage);
            Assert.Equal(10.0, ok.Value.Tier3WinningsShare);

            Assert.False(ok.Value.AllowMultiPrize);
            Assert.Equal("nearest", ok.Value.SplitResolution);
        }
        else
        {
            Assert.Fail("Load Config Failed");
        }
    }
}
