using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.UnitTests
{
    [TestFixture]
    public class CardNameLookupTests
    {
        private readonly ICardNameLookup<TestCard> _cardNameLookup = new CardNameLookup<TestCard>(CreateAllCardsTestList().AsQueryable());

        [Test]
        public void SmokeTest()
        {
           //Act
            var result = _cardNameLookup.FindCardByName("lim d l s vault");

            //Assert
            Assert.That(result.ResultObject.Name, Is.EqualTo("Lim-Dûl's Vault"));
        }

        [TestCase("dack fayden", "Dack Fayden")]
        [TestCase("fire ice", "Fire // Ice")]
        [TestCase("flame kin zealot", "Flame-Kin Zealot")]
        [TestCase("golgari grave troll", "Golgari Grave-Troll")]
        [TestCase("lim d l s vault", "Lim-Dûl's Vault")]
        [TestCase("ther vial", "Aether Vial")]
        [TestCase("man o war", "Man-o'-War")]
        [TestCase("elspeth knight errant", "Elspeth, Knight-Errant")]
        [TestCase("mental misstep", "Mental Misstep")]
        [TestCase("kiki jiki mirror breaker", "Kiki-Jiki, Mirror Breaker")]
        [TestCase("rune of protection red", "Rune of Protection: Red")]
        [TestCase("jaya ballard task mage", "Jaya Ballard, Task Mage")]
        [TestCase("thersnipe", "Aethersnipe")]
        [TestCase("gods eye gate to the reikai", "Gods' Eye, Gate to the Reikai")]
        [TestCase("j tun grunt", "Jötun Grunt")]
        [TestCase("eight and a half tails", "Eight-and-a-Half-Tails")]
        [TestCase("elspeth sun s champion", "Elspeth, Sun's Champion")]
        [TestCase("slayers stronghold", "Slayers' Stronghold")]
        [TestCase("wear tear", "Wear // Tear")]
        [TestCase("raider's spoils", "Raiders' Spoils")]
        [TestCase("anafenza kin tree spirit", "Anafenza, Kin-Tree Spirit")]
        [TestCase("arc slogger", "Arc-Slogger")]
        [TestCase("therspouts", "Aetherspouts")]
        [TestCase("vedalken thermage", "Vedalken Aethermage")]
        [TestCase("night of souls betrayal", "Night of Souls' Betrayal")]
        [TestCase("king macar the gold cursed", "King Macar, the Gold-Cursed")]
        [TestCase("vitu ghazi the city tree", "Vitu-Ghazi, the City-Tree")]
        [TestCase("s ance", "Séance")]
        [TestCase("fa adiyah seer ", "Fa'adiyah Seer")]
        [TestCase("circle of protection black", "Circle of Protection: Black")]
        [TestCase("will o the wisp", "Will-o'-the-Wisp")]
        [TestCase("scion of vitu ghazi", "Scion of Vitu-Ghazi")]
        [TestCase("scion of vitu ghazi", "Scion of Vitu-Ghazi")]
        [TestCase("silvergill adep", "Silvergill Adept")]
        [TestCase("snapc", "Snapcaster Mage")]
        [TestCase("gideon ally of zendikar", "Gideon, Ally of Zendikar")]
        [TestCase("mina and denn wildborn", "Mina and Denn, Wildborn")]
        public void FindCardTests(string searchCardName, string actualName)
        {
            //Arrange           
            TestCard findCardByName = _cardNameLookup.FindCardByName(searchCardName).ResultObject;
            Assert.That(findCardByName, Is.Not.Null.And.Property("Name").EqualTo(actualName));
        }

        [TestCase("  ")]
        [TestCase(" NonExistingName ")]
        public void FindCardReturnsNullWhenNotFoundTests(string searchCardName)
        {
            var nameLookupResult = _cardNameLookup.FindCardByName(searchCardName);            
            Assert.That(nameLookupResult, Is.Not.Null.And.Property("MatchResult").EqualTo(LookupMatchResult.NoMatch).And.Property("ResultObject").EqualTo(null));
        }

        private static List<TestCard> CreateAllCardsTestList()
        {
            return new List<TestCard>
            {
                new TestCard {Name = "Dack Fayden"},
                new TestCard {Name = "Fire // Ice"},
                new TestCard {Name = "Flame-Kin Zealot"},
                new TestCard {Name = "Golgari Grave-Troll"},
                new TestCard {Name = "Lim-Dûl's Vault"},
                new TestCard {Name = "Aether Vial"},
                new TestCard {Name = "Man-o'-War"},
                new TestCard {Name = "Elspeth, Knight-Errant"},
                new TestCard {Name = "Mental Misstep"},
                new TestCard {Name = "Kiki-Jiki, Mirror Breaker"},
                new TestCard {Name = "Rune of Protection: Red"},
                new TestCard {Name = "Jaya Ballard, Task Mage"},
                new TestCard {Name = "Aethersnipe"},
                new TestCard {Name = "Gods' Eye, Gate to the Reikai"},
                new TestCard {Name = "Jötun Grunt"},
                new TestCard {Name = "Eight-and-a-Half-Tails"},
                new TestCard {Name = "Elspeth, Sun's Champion"},
                new TestCard {Name = "Slayers' Stronghold"},
                new TestCard {Name = "Wear // Tear"},
                new TestCard {Name = "Raiders' Spoils"},
                new TestCard {Name = "Anafenza, Kin-Tree Spirit"},
                new TestCard {Name = "Arc-Slogger"},
                new TestCard {Name = "Aetherspouts"},
                new TestCard {Name = "Vedalken Aethermage"},
                new TestCard {Name = "Night of Souls' Betrayal"},
                new TestCard {Name = "King Macar, the Gold-Cursed"},
                new TestCard {Name = "Vitu-Ghazi, the City-Tree"},
                new TestCard {Name = "Séance"},
                new TestCard {Name = "Fa'adiyah Seer"},
                new TestCard {Name = "Circle of Protection: Black"},
                new TestCard {Name = "Will-o'-the-Wisp"},
                new TestCard {Name = "Scion of Vitu-Ghazi"},
                new TestCard {Name = "Scion of Vitu-Ghazi"},
                new TestCard {Name = "Silvergill Adept"},
                new TestCard {Name = "Snapcaster Mage"},
                new TestCard {Name = "Gideon, Ally of Zendikar"},
                new TestCard {Name = "Mina and Denn, Wildborn"}
            };
        }

        private class TestCard : IHasName
        {
            public string Name { get; set; }
        }
    }
}
