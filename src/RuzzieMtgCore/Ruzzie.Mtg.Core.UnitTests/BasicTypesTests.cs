using NUnit.Framework;

namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class BasicTypesTests
    {
        [TestCase("Artifact Creature — Soldier", BasicType.Artifact | BasicType.Creature)]
        [TestCase("Artifact", BasicType.Artifact | BasicType.Artifact)]
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
        public void BasicTypesParsingTests(string types, BasicType expectedBasicType)
        {
            Assert.That(BasicTypes.From(types), Is.EqualTo(expectedBasicType));
        }

        [TestCase(BasicType.Artifact | BasicType.Creature, BasicType.Artifact)]
        [TestCase(BasicType.Artifact | BasicType.Creature, BasicType.Creature)]
        [TestCase(BasicType.Creature, BasicType.Creature | BasicType.Artifact)]
        public void ContainsBasicType(BasicType input, BasicType shouldContain)
        {
            Assert.That(BasicTypes.ContainsBasicType(input,shouldContain), Is.True);
        }
    }
}