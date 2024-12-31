using Gambling.Library;
using Gambling.Library.Games;

namespace Gambling.API;

public class GameInputDto
{
    public required string ServerSeed { get; set; }
    public required string ClientSeed { get; set; }
    public long Nonce { get; set; } = 0;
    public required string InitialBalance { get; set; }
    public Strategies Strategy { get; set; } = Strategies.ModifiedMartingale;
    public GablingGame Game { get; set; } = GablingGame.Plinko;
}