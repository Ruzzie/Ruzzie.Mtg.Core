using FluentAssertions;
using NUnit.Framework;

namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class BasicTypesTests
    {
        [TestCase("Artifact Creature — Soldier", BasicType.Artifact | BasicType.Creature)]
        [TestCase("Artifact", BasicType.Artifact)]
        [TestCase("Artifact   \n", BasicType.Artifact)]
        [TestCase("Artifact   \n - \r\n", BasicType.Artifact)]
        [TestCase("Artifact - Creature", BasicType.Artifact | BasicType.Creature)]
        [TestCase("Legendary Artifact — Equipment", BasicType.Artifact)]
        [TestCase("Legendary Creature — Eldrazi", BasicType.Creature)]
        [TestCase("Land", BasicType.Land)]
        [TestCase("Basic Land", BasicType.BasicLand)]
        [TestCase("Enchantment", BasicType.Enchantment)]
        [TestCase("World Enchantment", BasicType.Enchantment)]
        [TestCase("Enchantment — Aura", BasicType.Enchantment)]
        [TestCase("Tribal Sorcery — Rogue", BasicType.Sorcery)]
        [TestCase("Sorcery", BasicType.Sorcery)]
        [TestCase("Artifact Land", BasicType.Artifact | BasicType.Land)]
        [TestCase("Enchantment Artifact", BasicType.Artifact | BasicType.Enchantment)]
        [TestCase("Enchantment Creature", BasicType.Enchantment | BasicType.Creature)]
        [TestCase("Land Creature", BasicType.Land | BasicType.Creature)]
        [TestCase("Instant", BasicType.Instant)]
        [TestCase("Instant — Arcane", BasicType.Instant)]
        [TestCase("Legendary Enchantment Creature — God", BasicType.Enchantment | BasicType.Creature)]
        [TestCase("Planeswalker — Liliana", BasicType.Planeswalker)]
        [TestCase("Basic Snow Land — Plains", BasicType.BasicLand)]
        [TestCase("Basic Snow Land   —  Plains  ", BasicType.BasicLand)]
        [TestCase("Basic Snow Land   —  Plains —  Plains ", BasicType.BasicLand)]
        public void BasicTypesParsingTests(string types, BasicType expectedBasicType)
        {
            BasicTypes.From(types).Should().Be(expectedBasicType);
        }

        [TestCase(BasicType.Artifact | BasicType.Creature, BasicType.Artifact)]
        [TestCase(BasicType.Artifact | BasicType.Creature, BasicType.Creature)]
        [TestCase(BasicType.Creature, BasicType.Creature | BasicType.Artifact)]
        public void ContainsAnyBasicType(BasicType input, BasicType shouldContain)
        {
            input.ContainsAnyBasicType(shouldContain).Should().BeTrue();
        }

        [TestCase(BasicType.Artifact | BasicType.Creature, BasicType.Artifact, true)]
        [TestCase(BasicType.Artifact | BasicType.Creature, BasicType.Creature, true)]
        [TestCase(BasicType.Creature, BasicType.Creature | BasicType.Artifact,false)]
        public void ContainsAllBasicType(BasicType input, BasicType typeToCheck, bool expected)
        {
            input.ContainsAllBasicType(typeToCheck).Should().Be(expected);
        }

        [TestCase(BasicType.Artifact | BasicType.Creature, false)]
        [TestCase(BasicType.Artifact | BasicType.Land, false)]
        [TestCase(BasicType.Artifact | BasicType.BasicLand, true)]
        [TestCase(BasicType.Land | BasicType.BasicLand, true)]
        [TestCase(BasicType.Land, true)]
        [TestCase(BasicType.None, false)]
        public void IsOnlyLandOrBasicLandType(BasicType input, bool expected)
        {
            input.IsOnlyLandOrBasicLandType().Should().Be(expected);
        }
    }
}