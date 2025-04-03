using System.Diagnostics;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Lotto.Config;

public record ConfigLoadErrorResult<LotteryConfiguration> : ErrorResult<LotteryConfiguration>;

public enum ConfigError
{
    ConfigFileNotFound,
    ConfigSchemaFileNotFound,
}

public class ConfigService
{
    private const string CONFIG_FILE_PATH = "config.json";
    private const string CONFIG_SCHEMA_PATH = "config_schema.json";
    private const string CONFIG_FILE_PATH_ERROR = "Config file not found.";
    private const string CONFIG_SCHEMA_PATH_ERROR = "Config schema file not found.";

    public static Result<LotteryConfiguration> LoadAndValidateLotteryConfig()
    {
        Debug.Assert(File.Exists(CONFIG_FILE_PATH), CONFIG_FILE_PATH_ERROR);
        Debug.Assert(File.Exists(CONFIG_SCHEMA_PATH), CONFIG_SCHEMA_PATH_ERROR);
        
        try 
        {
            string configJson = File.ReadAllText(CONFIG_FILE_PATH);        
            string schemaJson = File.ReadAllText(CONFIG_SCHEMA_PATH);        

            JSchema schema = JSchema.Parse(schemaJson);
            JObject configObject = JObject.Parse(configJson);

            Debug.Assert(configObject.IsValid(schema, out IList<string> validationErrors), $"Config JSON is invalid: {string.Join(", ", validationErrors)}");
            
            return new OkResult<LotteryConfiguration>(JsonSerializer.Deserialize<LotteryConfiguration>(configJson));
        }
        catch (Exception ex)
        {
            Debug.Assert(false, $"Failed to load game con: {ex.Message}");
            return new ConfigLoadErrorResult<LotteryConfiguration>();
        }
    }
}