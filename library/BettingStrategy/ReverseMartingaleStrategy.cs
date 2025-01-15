
namespace Gambling.Library;

public class ReverseMartingaleStrategy : IBettingStrategy
{
    private readonly Strategies _strategy = Strategies.ReverseMartingaleParoli;

    public Bet? Decide(Bet bet, decimal startingBetAmount, decimal totalBalance)
    {
        Bet newBet;

        if (bet.BetResult == BetResult.Won)
        {
            newBet = new(bet.Amount * 2, bet.Strategy)
            {
                BetResult = BetResult.NotPlayed,
                RawBetResult = null,
            };
        }
        else // LOSS
        {
            newBet = new(startingBetAmount, bet.Strategy)
            {
                BetResult = BetResult.NotPlayed,
                RawBetResult = null,
            };
        }

        if (newBet.Amount <= totalBalance)
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
