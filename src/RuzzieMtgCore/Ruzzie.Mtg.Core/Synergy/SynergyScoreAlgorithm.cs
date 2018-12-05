using System;

namespace Ruzzie.Mtg.Core.Synergy
{
    /// <summary>
    /// Contains methods to assign and increment a score for card synergy.
    /// </summary>
    public static class SynergyScoreAlgorithm
    {
        /// <summary>
        /// The epsilon
        /// </summary>
        public const float Epsilon = 0.00001F;

        private const int MaxScore = 10;

        /// <summary>
        /// Determines whether this instance is zero within the tolerance of <see cref="Epsilon"/>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is zero; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsZero(this float value)
        {
            return Math.Abs(value) < Epsilon;
        }

        /// <summary>
        /// Increments the synergy score on a curve to a maximum of 10f. With an increment factor of 1f. The first increment will increase the score by 1/4. And will continue to do so. The more increments, the slower the score will grow.
        /// For example. you start with a score of 1 and increment it a 100 times and compare it to 1 incremented 200 times. That difference will be smaller that calling increment 1 and 2 times.
        /// </summary>
        /// <param name="currentScore">The current score.</param>        
        /// <returns>The new score</returns>   
        public static float IncrementSynergyScore(this float currentScore)
        {
            return IncrementSynergyScore(currentScore, 1f);
        }

        /// <summary>
        /// Increments the synergy score on a curve to a maximum of 10f. The first increment will increase the score by 1/4. And will continue to do so. The more increments, the slower the score will grow.
        /// For example. you start with a score of 1 and increment it a 100 times and compare it to 1 incremented 200 times. That difference will be smaller that calling increment 1 and 2 times.
        /// </summary>
        /// <param name="currentScore">The current score.</param>
        /// <param name="incrementFactor">The increment value.</param>
        /// <returns>The new score</returns>   
        public static float IncrementSynergyScore(this float currentScore, ConstrainedValue<float, BetweenZeroAndOne> incrementFactor)
        {
            return IncrementSynergyScore(currentScore, incrementFactor.Value);
        }
       
        private static float IncrementSynergyScore(this float currentScore, float incrementFactor)
        {          
            if (currentScore.IsZero())
            {
                return incrementFactor;
            }

            if (incrementFactor.IsZero())
            {
                if (currentScore < MaxScore)
                {
                    return currentScore;
                }

                return MaxScore;
            }

            if (float.IsPositiveInfinity(currentScore) || float.IsNegativeInfinity(currentScore))
            {
                return MaxScore;
            }

            float totalIncrement = ((incrementFactor * 0.25F) / currentScore);

            if (totalIncrement > incrementFactor && (currentScore + incrementFactor < MaxScore))
            {
                return currentScore + incrementFactor;
            }

            float newScore = currentScore;

            if (currentScore < MaxScore)
            {
                newScore = currentScore + totalIncrement;
            }

            if (newScore >= MaxScore)
            {
                return MaxScore;
            }

            return newScore;
        }
    }
}
