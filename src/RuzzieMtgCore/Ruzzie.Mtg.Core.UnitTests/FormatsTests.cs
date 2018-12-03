using FluentAssertions;
using NUnit.Framework;

namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class FormatsTests
    {
        [Test]
        public void ContainsAnyFormatTrueTest()
        {
            (Format.Standard | Format.Modern).ContainsAnyFormat(Format.Modern).Should().BeTrue();
        }

        [Test]
        public void ContainsAnyFormatFalseTest()
        {
            (Format.Standard | Format.Legacy).ContainsAnyFormat(Format.Modern).Should().BeFalse();
        }

        [Test]
        public void ContainsAllFormatTrueTest()
        {
            (Format.Standard | Format.Legacy | Format.Modern).ContainsAllFormat(Format.Modern | Format.Legacy).Should().BeTrue();
        }

        [Test]
        public void ContainsAllFormatFalseTest()
        {
            (Format.Standard | Format.Legacy | Format.Modern).ContainsAllFormat(Format.Modern | Format.Penny).Should().BeFalse();
        }
    }
}
