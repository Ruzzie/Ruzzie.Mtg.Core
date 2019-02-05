using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Jil;
using NUnit.Framework;
using Ruzzie.Mtg.Core.Data;
using Ruzzie.Mtg.Core.Parsing;

namespace Ruzzie.Mtg.Core.UnitTests.Parsing
{
    [TestFixture]
    public class DeckCardNameCleanerTests
    {
        private IQueryable<IBasicCardProperties> _allDistinctCards;
        private CardNameLookup<IBasicCardProperties> _cardNameLookup;
        private IDeckCardNameCleaner<IBasicCardProperties> _cleaner;
        private  ICardNamesFromDeckTextParser _cardNamesFromDeckTextParser;
        private static readonly string BasePath = (AppContext.BaseDirectory + "\\Parsing\\");

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            var rawCards = JSON.Deserialize<List<IBasicCardProperties>>(File.ReadAllText(AppContext.BaseDirectory+"\\allcards.json"));
            _cardNamesFromDeckTextParser = new CardNamesFromDeckTextParser();
            _allDistinctCards =  rawCards.AsQueryable();
            _cardNameLookup = new CardNameLookup<IBasicCardProperties>(_allDistinctCards);
            _cleaner = new DeckCardNameCleaner<IBasicCardProperties>(_cardNameLookup);
            
        }
        
        [TestCase("mtgtop8deck_231142.txt")]
        [TestCase("mtgtop8TextDeckForTesting.txt")]
        [TestCase("mtgtop8deck_784225.txt")]
        [TestCase("mtgtop8deck_nastydeck_with_typos.txt")]
        [TestCase("mtgtop8deck_285946.txt")]
        [TestCase("mtgtop8deck_withae_test01.txt")]
        [TestCase("mtgtop8deck_withae_test02.txt")]
        public void SmokeTest(string testFile)
        {
            //Arange            
            var parseResult = _cardNamesFromDeckTextParser.Parse(File.ReadAllText(BasePath+testFile, Encoding.UTF8));

            //Clean up card names, by finding and matching them to the table, and uppercase the names
            //Fail hard when a card is not found
            //Act
            var cleanedCards = _cleaner.CleanUpCards(parseResult.Cards);

            //Assert
            Assert.That(cleanedCards.Count, Is.GreaterThan(0));
        }

        [Test]
        public void NonLandCardsWithAMultipleOf8ShouldBeCappedAtFour()
        {
            //Arrange
            string textDeck = @"8 Boon Satyr";
            var parseResult = _cardNamesFromDeckTextParser.Parse(textDeck);

            //Act
            var cleanedCards = _cleaner.CleanUpCards(parseResult.Cards);

            //Assert
            Assert.That(cleanedCards[0].Count ,Is.EqualTo(4));
            Assert.That(cleanedCards[0].Name, Is.EqualTo("Boon Satyr"));
        }       

        [Test]
        public void EAccentCardsShouldBeFound()
        {
            //Arrange
            string textDeck = @"2 S�ance";
            char invalidEAcc = '�';
            Console.WriteLine((int) invalidEAcc);
            var parseResult = _cardNamesFromDeckTextParser.Parse(textDeck);

            //Act
            var cleanedCards = _cleaner.CleanUpCards(parseResult.Cards);

            //Assert
            Assert.That(cleanedCards[0].Count, Is.EqualTo(2));
            Assert.That(cleanedCards[0].Name, Is.EqualTo("Séance"));
        }

        [Test]
        public void UnknownCardShouldBeIgnored()
        {
            //Arrange
            string textDeck = @"2 unknown card
4 Giant Growth";                        
            var parseResult = _cardNamesFromDeckTextParser.Parse(textDeck);

            //Act
            var cleanedCards = _cleaner.CleanUpCards(parseResult.Cards);

            //Assert
            Assert.That(cleanedCards[0].Count, Is.EqualTo(4));
            cleanedCards.Count.Should().Be(1);
        }

        [Test]
        public void CardWithBasicLandTypeNameShouldBeABasicLand()
        {
            //Arrange
            string textDeck = @"21 Forest Basic Land That ASdklj lkjsd lkj";                        
            var parseResult = _cardNamesFromDeckTextParser.Parse(textDeck);

            //Act
            var cleanedCards = _cleaner.CleanUpCards(parseResult.Cards);

            //Assert
            Assert.That(cleanedCards[0].Count, Is.EqualTo(21));
            cleanedCards.Count.Should().Be(1);
        }

        [Test]
        public void CardCountGreaterThanFourAndLessThanEightShouldBeHalvedWhenItIsntABasicLandCardNameMatch()
        {
            //Arrange
            string textDeck = "9 Island Sancuary\n 6 Ponder";
            var parseResult = _cardNamesFromDeckTextParser.Parse(textDeck);

            //Act
            var cleanedCards = _cleaner.CleanUpCards(parseResult.Cards);

            //Assert
            parseResult.ResultCode.Should().Be(ParseResultCode.Success, parseResult.Message);
            cleanedCards.Should()
                .Contain(item => item.Name == "Island" && item.Count == 9)
                .And
                .Contain(item => item.Name == "Ponder" && item.Count == 3);                     
        }

        [Test]
        public void WhenCardIsMoreThanFourAndEightAndNotAMultipleOfFourAndContainsBasicLandNameCardNameTreatItAsABasicLand()
        {
            string textDeck = "25 Island Sancuary\n1 Talrand, Sky Summoner";
            var parseResult = _cardNamesFromDeckTextParser.Parse(textDeck);

            //Act
            var cleanedCards = _cleaner.CleanUpCards(parseResult.Cards);

            //Assert
            cleanedCards.Should()
                .Contain(item => item.Name == "Island" && item.Count == 25)
                .And
                .HaveCount(2);
        }

        [Test]
        public void ConsolidateDuplicateEntriesTest()
        {
            //Arrange
            var parseResult = _cardNamesFromDeckTextParser.Parse(File.ReadAllText(BasePath+"mtgtop8deck_deck_with_duplicate_entries.txt"));
            
            //Act
            var cleanedCards = _cleaner.CleanUpCards(parseResult.Cards);

            //Assert
            cleanedCards.Should()
                .Contain(item => item.Name == "Turn // Burn" && item.Count == 4)
                .And
                .HaveCount(5);
        }

        [Test]
        public void ReturnsADeckWithSideBoard()
        {
            //Assert
            string cardsAsText = @"12 Plains
4 Spear of Heliod
4 Brave the Elements
4 Frontline Medic
4 Banisher Priest
4 Imposing Sovereign
4 Azorius Arrester
4 Dryad Militant
4 Daring Skyjek
4 Boros Elite
4 Soldier of the Pantheon
4 Loyal Pegasus
4 Judge's Familiar
Sideboard
4 Relic of Progenitus
3 Karakas
4 Grafdigger's Cage
2 Crucible of Worlds
2 Coercive Portal
";
            var parseResult = _cardNamesFromDeckTextParser.Parse(cardsAsText, DeckTextParseOptions.WithSideBoard);

            //Act
            var cleanedDeckCards = _cleaner.CleanUpCards(parseResult.Cards, parseResult.Sideboard);

            //Assert
            cleanedDeckCards.Mainboard.Sum(x => x.Count).Should().Be(60);
            cleanedDeckCards.Sideboard.Sum(x => x.Count).Should().Be(15);            
        }
    }

    [TestFixture]
    public class ExportDeckCardsToPlainTextTests
    {
        [Test]
        public void SmokeTest()
        {
            //Arrange            
            var mainBoard = new List<IDeckCard<IBasicCardProperties>>();
            mainBoard.Add(new DeckCard<IBasicCardProperties>(new BasicCardPoco {Name = "Boros Elite"}, 2));
            mainBoard.Add(new DeckCard<IBasicCardProperties>(new BasicCardPoco {Name = "Boros Elite"}, 2));

            var sideBoard = new List<IDeckCard<IBasicCardProperties>>();
            sideBoard.Add(new DeckCard<IBasicCardProperties>(new BasicCardPoco {Name = "Karakas" }, 3));

            DeckCards<IBasicCardProperties> cards = new DeckCards<IBasicCardProperties>(mainBoard, sideBoard);
            IDeckExporter<string, IBasicCardProperties> deckExporter = new DeckExporterPlainText<IBasicCardProperties>();

            //Act
            string deckText = deckExporter.Export(cards);

            //Assert
            deckText.Should().Be("4 Boros Elite\r\nSideboard\r\n3 Karakas\r\n");
        }
    }

    public class BasicCardPoco : IBasicCardProperties
    {
        public string Name { get; set; }
        public double? Price { get; set; }
        public int Cmc { get; set; }
        public int ColorIdentity { get; set; }
        public int BasicType { get; set; }
        public string Types { get; set; }
        public double? Rating { get; set; }
        public int Legality { get; set; }
        public string ManaCost { get; set; }
    }
}