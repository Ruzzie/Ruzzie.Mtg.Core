using System;
using FsCheck;
using NUnit.Framework;
using Ruzzie.Mtg.Core.Synergy;

namespace Ruzzie.Mtg.Core.UnitTests.Synergy
{

    [TestFixture]
    public class ConstrainedValueTests
    {
        [FsCheck.NUnit.Property(Arbitrary = new[] {typeof(ArbitraryFloatBetweenZeroAndOne)})]
        public Property QuickCheckBetweenZeroAndOne(float value)
        {
            var constrainedValue = new ConstrainedValue<float, BetweenZeroAndOne>(value);
            Func<bool> check = () => constrainedValue == value;
            return check.ToProperty();
        }
    }
}
