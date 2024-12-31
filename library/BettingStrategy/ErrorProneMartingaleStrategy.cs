namespace Gambling.Library;

public class ErrorProneMartingaleStrategy : IBettingStrategy
{
    private readonly Strategies _strategy = Strategies.Martingale;

    // Error probabilities
    private const double FORGET_RESET_PROBABILITY = 0.01;  // 1% chance to forget resetting after win
    private const double WRONG_CALCULATION_PROBABILITY = 0.02;  // 2% chance to miscalculate
    private const double CALCULATION_ERROR_RANGE = 0.1;  // ±10% error in calculations
    private const double SKIP_DOUBLING_PROBABILITY = 0.015;  // 1.5% chance to skip doubling after loss

    public Bet? Decide(Bet bet, decimal startingBetAmount, decimal totalBalance)
    {
        Bet newBet;

        if (bet.BetResult == BetResult.Won)
        {
            // Determine bet amount after win
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
            // Calculate next bet amount
            decimal nextBetAmount = Random.Shared.NextDouble() < SKIP_DOUBLING_PROBABILITY
                ? bet.Amount  // Forgot to double
                : bet.Amount * 2;  // Correct doubling

            // Apply calculation error if it occurs
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

        // Round to 2 decimal places and check against balance
        newBet = new(Math.Round(newBet.Amount, 2), newBet.Strategy)
        {
            BetResult = newBet.BetResult,
            RawBetResult = newBet.RawBetResult
        };

        return newBet.Amount < totalBalance ? newBet : null;
    }

    public Strategies GetStrategy()
    {
        return _strategy;
    }
}