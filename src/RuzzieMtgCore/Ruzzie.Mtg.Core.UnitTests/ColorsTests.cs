using FluentAssertions;
using NUnit.Framework;

namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class ColorsTests
    {
        [TestCase("B", Color.B)]
        [TestCase("WUB", Color.B | Color.W | Color.U)]
        [TestCase("WUB ", Color.B | Color.W | Color.U)]
        [TestCase("", Color.Colorless)]
        [TestCase(null, Color.Colorless)]
        [TestCase("C", Color.Colorless)]
        public void ColorsIdentityStringToColorsIdentityEnumTest(string colorsIdentityString, Color expectedIdentity)
        {
            Color colorIdentity = Colors.From(colorsIdentityString);

            Assert.That(colorIdentity, Is.EqualTo(expectedIdentity));
        }

        [Test]
        public void OrShouldResultInTwoColors()
        {
            Color color = Color.B | Color.G;

            Assert.That(color.HasFlag(Color.B), Is.True);
            Assert.That(color.HasFlag(Color.G), Is.True);
        }

        [Test]
        public void CompareAllowedColorsToDisallowedColor()
        {
            Color allowedColors = Color.B | Color.G | Color.Colorless | Color.U | Color.W;
            Color cardWithDisallowedColor = Color.R;

            Assert.That(allowedColors.HasFlag(cardWithDisallowedColor), Is.False);
        }

        [Test]
        public void CompareAllowedColorsToColor()
        {
            Color allowedColors = Color.B | Color.G | Color.Colorless | Color.U | Color.W;
            Color cardWithDisallowedColor = Color.B | Color.U;

            Assert.That(allowedColors.HasFlag(cardWithDisallowedColor), Is.True);
        }

        [Test]
        public void CompareColors()
        {
            Color uw = Color.U | Color.W;
            Color wu = Color.W | Color.U;

            Assert.That(uw, Is.EqualTo(wu));
        }

        [TestCase(new string[] { }, Color.Colorless)]
        [TestCase(null, Color.Colorless)]
        [TestCase(new string[] { "U" }, Color.U)]
        [TestCase(new string[] { "U", "W" }, Color.U | Color.W)]
        [TestCase(new string[] { "B", "G", null }, Color.B | Color.G)]
        [TestCase(new string[] { "B", "G", "" }, Color.B | Color.G)]
        [TestCase(new string[] { "B", "G", "Q" }, Color.B | Color.G)]
        [TestCase(new string[] { "U", "W", "A", "G", "L" }, Color.U | Color.W | Color.G)]
        public void FromStringArrayTest(string[] values, Color expected)
        {
            Assert.That(Colors.From(values), Is.EqualTo(expected));
        }

        [TestCase(Color.Colorless, false)]
        [TestCase(Color.W, false)]
        [TestCase(Color.W | Color.B, true)]
        [TestCase(Color.W | Color.B | Color.G, true)]
        public void HasMoreThanOneColorTest(Color value, bool expected)
        {
            Assert.That(value.HasMoreThanOneColor(), Is.EqualTo(expected));
        }

        [TestCase(Color.Colorless, Color.Colorless, false)]
        [TestCase(Color.Colorless, Color.W, false)]
        [TestCase(Color.W, Color.Colorless, false)]
        [TestCase(Color.W, Color.W, true)]
        [TestCase(Color.W | Color.B, Color.W, true)]
        [TestCase(Color.W, Color.G, false)]
        [TestCase(Color.W | Color.B, Color.G, false)]
        [TestCase(Color.G | Color.B, Color.G | Color.U, true)]
        public void ContainsAnyColorTest(Color source, Color target, bool expected)
        {
            source.ContainsAnyColor(target).Should().Be(expected);
        }

        [TestCase(Color.Colorless, Color.Colorless, true)]
        [TestCase(Color.Colorless, Color.W, false)]
        [TestCase(Color.W, Color.Colorless, true)]
        [TestCase(Color.W, Color.W, true)]
        [TestCase(Color.W | Color.B, Color.W, true)]
        [TestCase(Color.W, Color.G, false)]
        [TestCase(Color.W | Color.B, Color.G, false)]
        [TestCase(Color.G | Color.B, Color.G | Color.U, false)]
        public void ContainsAllColorTest(Color source, Color target, bool expected)
        {
            source.ContainsAllColor(target).Should().Be(expected);
        }

        [TestCase(Color.Colorless, 0, false)]
        [TestCase(Color.Colorless, 1, false)]
        [TestCase(Color.W | Color.G, 1, true)]
        [TestCase(Color.W | Color.G, 2, false)]
        [TestCase(Color.W | Color.G | Color.R, 2, true)]
        [TestCase(Color.W | Color.G | Color.R, 3, false)]
        public void HasMoreThanColorCountTest(Color color, int count, bool expected)
        {
            Assert.That(color.HasMoreThanColorCount(count), Is.EqualTo(expected));
        }

        [TestCase(Color.B, "B")]
        [TestCase(Color.B | Color.G, "B, G")]
        [TestCase(Color.B | Color.G | Color.U, "U, B, G")]
        public void CardColorsToStringTest(Color input, string expectedValue)
        {
            Assert.That("" + input, Is.EqualTo(expectedValue));
        }

        [TestCase(Color.Colorless, 0)]
        [TestCase(Color.R, 1)]
        [TestCase(Color.R | Color.B, 2)]
        [TestCase(Color.Colorless | Color.R | Color.B | Color.G | Color.U | Color.W, 5)]
        public void GetNumberOfColorsTest(Color input, int expectedCount)
        {
            input.GetNumberOfColors().Should().Be(expectedCount);
        }
    }
}