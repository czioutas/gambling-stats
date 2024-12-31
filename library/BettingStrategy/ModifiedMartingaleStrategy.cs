namespace Gambling.Library;

public class ModifiedMartingaleStrategy : IBettingStrategy
{
    private readonly Strategies _strategy = Strategies.ModifiedMartingale;

    public Bet? Decide(Bet bet, decimal startingBetAmount, decimal totalBalance)
    {
        Bet newBet;

        if (bet.BetResult == BetResult.Stalemate)
        {
            newBet = new(bet.Amount, bet.Strategy)
            {
                BetResult = BetResult.NotPlayed,
                RawBetResult = null,
            };
        }
        else if (bet.BetResult == BetResult.Won)
        {
            newBet = new(startingBetAmount, bet.Strategy)
            {
                BetResult = BetResult.NotPlayed,
                RawBetResult = null,
            };
        }
        else // LOSS
        {
            newBet = new(bet.Amount * 2, bet.Strategy)
            {
                BetResult = BetResult.NotPlayed,
                RawBetResult = null,
            };
        }

        if (newBet.Amount < totalBalance)
        {
            return newBet;
        }
        else
        {
            return null;
        }
    }

    public Strategies GetStrategy()
    {
        return _strategy;
    }
}

