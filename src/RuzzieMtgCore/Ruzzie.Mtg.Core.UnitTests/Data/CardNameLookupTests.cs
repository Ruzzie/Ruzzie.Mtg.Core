using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Ruzzie.Common.Hashing;
using Ruzzie.FuzzyStrings;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.UnitTests.Data
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

        [Test]
        public void CtorTest()
        {
           var typeWithIHasAllCardsProperty = new TestCardContainer();
            // ReSharper disable once ObjectCreationAsStatement
           new CardNameLookup<TestCard>(typeWithIHasAllCardsProperty);
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
        [TestCase("Turn / Burn", "Turn // Burn")]
        [TestCase("Rakdos Return", "Rakdos's Return")]
        [TestCase("Rakdos's Return", "Rakdos's Return")]
        [TestCase("Rakdos's Returns", "Rakdos's Return")]
        //some keymatches
        [TestCase("FAR __ AWAY", "Far // Away")]
        [TestCase("GIANT GROWTH", "Giant Growth")]
        //Some Fuzzy matching tests
        [TestCase("Raakdoos Returns", "Rakdos's Return")]
        [TestCase("Aethersprouts", "Aetherspouts")]
        [TestCase("jotun grunt", "Jötun Grunt")]
        [TestCase("men o war", "Man-o'-War")]        
        public void FindCardTests(string searchCardName, string actualName)
        {
            //Arrange           
            var nameLookupResult = _cardNameLookup.FindCardByName(searchCardName);
            TestCard findCardByName = nameLookupResult.ResultObject;
            Assert.That(findCardByName, Is.Not.Null.And.Property("Name").EqualTo(actualName));

            nameLookupResult.MatchProbability.Should().BeGreaterOrEqualTo(0.75);
        }

        [Test]
        [TestCase("Giant Gowth", "Giant Growth")]
        [TestCase("Giant Ortoise", "Giant Tortoise")]
        public void FuzzyMatchTestAndOrdering(string searchCardName, string actualName)
        {
            TestCard findCardByName = _cardNameLookup.FindCardByName(searchCardName, 0.85D).ResultObject;
            Assert.That(findCardByName, Is.Not.Null.And.Property("Name").EqualTo(actualName));
        }


        [Test]
        public void StripTest()
        {
            string strippedOne = "Men-o'-War".RemoveSpecialCharacters();
            Console.WriteLine("Stripped: "+strippedOne);
            Console.WriteLine(strippedOne.FuzzyMatch("Man-o'-War".RemoveSpecialCharacters(), false));
        }

        [TestCase("  ")]
        [TestCase(" NonExistingName ")]
        public void FindCardReturnsNullWhenNotFoundTests(string searchCardName)
        {
            var nameLookupResult = _cardNameLookup.FindCardByName(searchCardName);            
            Assert.That(nameLookupResult, Is.Not.Null.And.Property("MatchResult").EqualTo(LookupMatchResult.NoMatch).And.Property("ResultObject").EqualTo(null));
        }

        [Test]
        public void HashTest()
        {
            var cardName = KnownSynonyms.GetSynonym("fa adiyah seer");
            var dataSource = new CardNameLookupDataSource<TestCard>(CreateAllCardsTestList().AsQueryable());


            var algo = new FNV1AHashAlgorithm64();
            var hash = algo.HashStringCaseInsensitive(cardName.CreateValidUpperCaseKeyForString());

            dataSource.HashLookup[hash].Should().NotBeNull();
            TestCard foundCard;
            dataSource.HashLookup.TryGetValue(hash, out foundCard).Should().BeTrue();

            hash.Should().Be(algo.HashStringCaseInsensitive(cardName.CreateValidUpperCaseKeyForString()));
        }
        
        [TestCase("dack", 1)]
        [TestCase("a", 10)]
        [TestCase("ae", 4)]
        [TestCase("Slogger", 1)]
        [TestCase("seance", 1)]
        [TestCase("Giant", 2)]
        [TestCase("Giant  ", 2)]
        [TestCase("asfasfjsdlkgjslkgjslkdgjdflkgjk", 0)]
        public void MultipleProbableResultsWhenNoExactResultIsFound(string search, int expectedResults)
        {
            var results = _cardNameLookup.LookupCardName(search, 0.33, 10);

            results.Count().Should().Be(expectedResults);
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
                new TestCard {Name = "Scion of Vitu-Ghazi"},//nice to see if distinct selection works
                new TestCard {Name = "Silvergill Adept"},
                new TestCard {Name = "Snapcaster Mage"},
                new TestCard {Name = "Gideon, Ally of Zendikar"},
                new TestCard {Name = "Mina and Denn, Wildborn"},
                new TestCard {Name = "Turn // Burn"},
                new TestCard {Name = "Rakdos's Return"},
                new TestCard {Name = "Giant Tortoise"},
                new TestCard {Name = "Giant Growth"},
                new TestCard {Name = "Far // Away"}
            };
        }

        public class TestCard : IHasName
        {
            public string Name { get; set; }
        }

        public class TestCardContainer : IHasQuerableAllCards<TestCard>
        {
            public IQueryable<TestCard> AllCards { get { return CreateAllCardsTestList().AsQueryable(); } }
        }
    }
}
