﻿using NUnit.Framework;

namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class GenerateBidirectonalKeyForTwoStringComparisonsTests
    {
        [TestCase("A", "A")]
        [TestCase("Urborg Justice", "Skyshroud Forest")]
        [TestCase("Urborg Justice", "Skyshroud Forest")]
        [TestCase("A", "B")]
        [TestCase("Our Market Research Shows That Players Like Really Long Card Names So We Made this Card to Have the Absolute Longest Card Name Ever Elemental",
            "Rhox Maulers")]
        public void TestBidirectonality(string a, string b)
        {
            ulong firstKey = a.GenerateBidirectonalKeyForTwoStringComparisons(b);
            ulong secondKey = b.GenerateBidirectonalKeyForTwoStringComparisons(a);

            Assert.That(firstKey, Is.EqualTo(secondKey));
        }

        [TestCase("Oloro, Ageless Ascetic", "Oloro, Ageless Ascetic", "Kraken's Eye", "Kraken's Eye")]
        public void UniqueTest(string a1, string a2, string b1, string b2)
        {
            ulong firstKey = a1.GenerateBidirectonalKeyForTwoStringComparisons(a2);
            ulong secondKey = b1.GenerateBidirectonalKeyForTwoStringComparisons(b2);

            Assert.That(firstKey, Is.Not.EqualTo(secondKey));
        }

        [TestCase("A", "A", 6490070733419165712UL)]
        [TestCase("Urborg Justice", "Skyshroud Forest", 7481129173139490781UL)]
        [TestCase("Oloro, Ageless Ascetic", "Oloro, Ageless Ascetic", 4628074897764172800UL)]
        public void ValueRegressionTest(string a, string b, ulong expected)
        {
            ulong firstKey = a.GenerateBidirectonalKeyForTwoStringComparisons(b);

            Assert.That(firstKey, Is.EqualTo(expected));
        }
    }
}