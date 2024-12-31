using System.Text.Json.Serialization;

namespace Gambling.Library;

public enum Strategies
{
    [JsonPropertyName("martingale")]
    Martingale,
    [JsonPropertyName("modifiedMartingale")]
    ModifiedMartingale,
    [JsonPropertyName("errorProneModifiedMartingale")]
    ErrorProneModifiedMartingale,
    [JsonPropertyName("errorProneMartingale")]
    ErrorProneMartingale
}

public interface IBettingStrategy
{
    Strategies GetStrategy();
    Bet? Decide(Bet bet, decimal startingBetAmount, decimal totalBalance);
}