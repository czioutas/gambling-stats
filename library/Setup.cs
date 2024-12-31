using Gambling.Library.Games;

namespace Gambling.Library;

public enum NextMove
{
    Play,
    Exit
}

public class Setup
{
    private IGamblingGame _game;
    private Gambler _gambler;
    private readonly Stats _stats;

    public Setup(IGamblingGame game, Gambler gambler)
    {
        _game = game;
        _gambler = gambler;
        _stats = new Stats();
    }

    public Stats Run()
    {
        int betId = 0;
        Bet? betBeforePlay = null;

        do
        {
            // we calculate the first bet based on the strategy
            betBeforePlay = _gambler.RunStrategy(betBeforePlay);

            // if we think the odds are again us we might not even play the first hand
            if (betBeforePlay is null)
            {
                return _stats;
            }

            betBeforePlay.Id = betId++;
            betBeforePlay.BalanceBeforeBet = _gambler.Balance;
            // if we have a bet, we let the game play it
            Bet betAfterPlay = _game.Play(betBeforePlay);  // now the Bet has a result

            betAfterPlay.BalanceAfterBet = _gambler.Balance = betAfterPlay.BalanceBeforeBet - betBeforePlay.Amount + betAfterPlay.Payout.Value;

            _stats.Bets.Add(betAfterPlay);
        }
        while (betBeforePlay is not null); // if we want to play again the loop continues

        return _stats;
    }
}