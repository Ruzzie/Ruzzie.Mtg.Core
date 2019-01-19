using System;

namespace Ruzzie.Mtg.Core.Synergy
{
    /// <summary>A constraint on a float that defines that the value should be between 0 and 1 (inclusive)</summary>
    public class BetweenZeroAndOne : IValueConstraint<float>
    {
        /// <inheritdoc />
        public bool IsWithinConstraint(float value)
        {            
            return value >= 0 && value <= 1f;
        }
    }
}