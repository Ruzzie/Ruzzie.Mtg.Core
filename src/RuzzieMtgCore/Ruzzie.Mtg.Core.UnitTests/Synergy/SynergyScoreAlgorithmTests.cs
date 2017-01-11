using NUnit.Framework;
using Ruzzie.Mtg.Core.Synergy;

namespace Ruzzie.Mtg.Core.UnitTests.Synergy
{
    [TestFixture]
    public class SynergyScoreAlgorithmTests
    {
        [TestCase(0.000009F, 0.99999F, 0.99999F)]
        [TestCase(0F, 0.99999F, 0.99999F)]
        [TestCase(0.99999F, 0.99999F, 1.24999F)]
        [TestCase(0.99999F, 0F, 0.99999F)]
        [TestCase(0.01F, 1F, 1.00999999f)]
        [TestCase(0F, 1F, 1F)]
        public void TestScoreIncrement(float score, float increment, float expected)
        {
            Assert.That(score.IncrementSynergyScore(increment), Is.EqualTo(expected));
        }

        [Test]
        public void VerySmallScoreValueShouldNotReturnLargeReturnValue()
        {
            Assert.That(0.0001F.IncrementSynergyScore(), Is.LessThanOrEqualTo(1.0001F));
        }

        [Test]
        public void ScoreCannotBeGreaterThat10()
        {
            Assert.That(10F.IncrementSynergyScore(), Is.LessThanOrEqualTo(10F));
        }
    }
}
