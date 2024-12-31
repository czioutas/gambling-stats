using System.Text.Json.Serialization;
using Gambling.Library;

namespace Gambling.Library.Games;

public enum GablingGame
{
    [JsonPropertyName("plinko")]
    Plinko
}

public interface IGamblingGame
{
    Bet Play(Bet bet);
}