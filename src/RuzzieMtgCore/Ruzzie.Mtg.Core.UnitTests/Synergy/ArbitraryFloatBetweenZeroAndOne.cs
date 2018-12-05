using System;
using FsCheck;

namespace Ruzzie.Mtg.Core.UnitTests.Synergy
{   
    public class ArbitraryFloatBetweenZeroAndOne : Arbitrary<float>
    {
        public override Gen<float> Generator
        {
            get { return Gen.Choose(0, Int32.MaxValue).Select(i => i > 0 ? 1f / i : i); }
        }

        /// <summary>
        /// Generates Float values between 0 and 1
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static Arbitrary<float> Float() {return new ArbitraryFloatBetweenZeroAndOne();}
    }
}