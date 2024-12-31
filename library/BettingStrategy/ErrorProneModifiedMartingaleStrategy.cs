namespace Gambling.Library;

public class ErrorProneModifiedMartingaleStrategy : IBettingStrategy
{
    private readonly Strategies _strategy = Strategies.ModifiedMartingale;

    // Error probabilities
    private const double FORGET_RESET_PROBABILITY = 0.01;  // 1% chance to forget resetting after win
    private const double WRONG_CALCULATION_PROBABILITY = 0.02;  // 2% chance to miscalculate
    private const double CALCULATION_ERROR_RANGE = 0.1;  // ±10% error in calculations
    private const double SKIP_DOUBLING_PROBABILITY = 0.015;  // 1.5% chance to skip doubling after loss

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
            decimal betAmount = Random.Shared.NextDouble() < FORGET_RESET_PROBABILITY
                ? bet.Amount  // Forgot to reset
                : startingBetAmount;  // Correct reset

            newBet = new(betAmount, bet.Strategy)
            {
                BetResult = BetResult.NotPlayed,
                RawBetResult = null,
            };
        }
        else // LOSS
        {
            decimal nextBetAmount = Random.Shared.NextDouble() < SKIP_DOUBLING_PROBABILITY
                ? bet.Amount  // Forgot to double
                : bet.Amount * 2;  // Correct doubling

            if (Random.Shared.NextDouble() < WRONG_CALCULATION_PROBABILITY)
            {
                double errorFactor = 1 + ((Random.Shared.NextDouble() * 2 - 1) * CALCULATION_ERROR_RANGE);
                nextBetAmount = (decimal)(double)(nextBetAmount * (decimal)errorFactor);
            }

            newBet = new(nextBetAmount, bet.Strategy)
            {
                BetResult = BetResult.NotPlayed,
                RawBetResult = null,
            };
        }

        newBet = new(Math.Round(newBet.Amount, 2), newBet.Strategy)
        {
            BetResult = newBet.BetResult,
            RawBetResult = newBet.RawBetResult
        };

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