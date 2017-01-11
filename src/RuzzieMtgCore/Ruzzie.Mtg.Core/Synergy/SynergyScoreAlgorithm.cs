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

        /// <summary>
        /// Determines whether this instance is zero whitin the tolerance of <see cref="Epsilon"/>
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
        /// Increments the synergy score.
        /// </summary>
        /// <param name="currentScore">The current score.</param>
        /// <param name="incrementFactor">The increment factor.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// incrementFactor should be a value between 0 and 1. Current value is: {incrementFactor}
        /// or
        /// Increment should be a value between 0 and 1. Current value is: {incrementFactor}
        /// </exception>
        public static float IncrementSynergyScore(this float currentScore, float incrementFactor = 1)
        {
            if (incrementFactor > 1)
            {
                throw new ArgumentException($"incrementFactor should be a value between 0 and 1. Current value is: {incrementFactor}", nameof(incrementFactor));
            }

            if (incrementFactor < 0)
            {
                throw new ArgumentException($"Increment should be a value between 0 and 1. Current value is: {incrementFactor}", nameof(incrementFactor));
            }

            if (currentScore.IsZero())
            {
                return incrementFactor;
            }

            if (incrementFactor.IsZero())
            {
                return currentScore;
            }

            float totalIncrement = ((incrementFactor * 0.25F) / currentScore);

            if (totalIncrement > incrementFactor)
            {
                return currentScore + incrementFactor;
            }

            float newScore = currentScore;

            if (currentScore < 10)
            {
                newScore = currentScore + totalIncrement;
            }

            return newScore;
        }
    }
}
