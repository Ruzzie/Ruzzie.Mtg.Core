using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using Ruzzie.Mtg.Core.Parsing;

namespace Ruzzie.Mtg.Core.UnitTests.Parsing
{
    [TestFixture]
    public class CardNamesFromDeckTextParserTests
    {
        private CardNamesFromDeckTextParser _parser;
        private static string BasePath = (AppContext.BaseDirectory + "\\Parsing\\");
        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            _parser = new CardNamesFromDeckTextParser();
        }


        [TestCase("mtgtop8deck_231142.txt")]
        public void SmokeTest(string filename)
        {
            CardParseResult cardParseResult = _parser.Parse(File.ReadAllText(BasePath+"mtgtop8deck_231142.txt"));
            
            Assert.That(cardParseResult.ResultCode, Is.EqualTo(ParseResultCode.Success));
            Assert.That(cardParseResult.Cards.Count, Is.GreaterThan(0));
        }
        [Test]
        public void CardListMustBeFilledWhenCorrectCardsAreGivenSingleLine()
        {
            string cardsAsText = @"4 Spear of Heliod";

            CardParseResult result = _parser.Parse(cardsAsText);

            Assert.That(result.Cards.Count, Is.EqualTo(1));
            Assert.That(result.Cards.Sum(item => item.Count), Is.EqualTo(4));
        }

        [Test]
        public void CardListMustBeFilledWhenCorrectCardsAreGivenMultiLine()
        {
            string cardsAsText = @"4 Spear of Heliod
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
4 Judge's Familiar";

            CardParseResult result = _parser.Parse(cardsAsText);

            Assert.That(result.Cards.Count, Is.EqualTo(12));
            Assert.That(result.Cards.Sum(item => item.Count), Is.EqualTo(48));
        }

        [Test]
        public void CardListMustNotContainSideboard()
        {
            string cardsAsText = @"4 Spear of Heliod
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
4 Boros Elite
4 Soldier of the Pantheon
4 Loyal Pegasus
";

            CardParseResult result = _parser.Parse(cardsAsText);

            Assert.That(result.Cards.Count, Is.EqualTo(12));
            Assert.That(result.Cards.Sum(item => item.Count), Is.EqualTo(48));
        }

        [Test]
        public void ResultMustBeFailedWithErrorMessageWhenCardNameIsEmpty()
        {
            string cardsAsText = @"4       ";

            CardParseResult result = _parser.Parse(cardsAsText);

            Assert.That(result.ResultCode, Is.EqualTo(ParseResultCode.Failed));
            Assert.That(result.Message, Is.EqualTo("Line \"4\" is not valid. Please check your spelling. "));
            Assert.That(result.Cards.Count, Is.EqualTo(0));
        }

        [Test]
        public void ResultMustBeFailedWithErrorMessageWhenSecondCardIsEmpty()
        {
            string cardsAsText = @"4 Spear of Heliod
4     ";

            CardParseResult result = _parser.Parse(cardsAsText);

            Assert.That(result.ResultCode, Is.EqualTo(ParseResultCode.Failed));
            Assert.That(result.Cards.Count, Is.EqualTo(1));
        }

        [Test]
        public void ReturnFailureForEmptyText()
        {
            CardParseResult result = _parser.Parse(string.Empty);

            Assert.That(result.ResultCode, Is.EqualTo(ParseResultCode.NotParsedEmptyText));
        }

        [Test]
        public void CardNameShouldBeTrimmed()
        {
            //Arrange
            string textDeck = "\t \t  8 Boon Satyr   \n\t 20    Island\n4\tGiant Growth\n\n\n\n\n";

            //Act          
            var parseResult = _parser.Parse(textDeck);

            //Assert
            Assert.That(parseResult.ResultCode, Is.EqualTo(ParseResultCode.Success));
            
            Assert.That(parseResult.Cards[0].Name, Is.EqualTo("Boon Satyr"));
            Assert.That(parseResult.Cards.Count, Is.EqualTo(3));
        }

        [Test]
        public void SideboardMustBeFilledWhenSideboardLineIsPresent()
        {
            string cardsAsText = @"4 Spear of Heliod
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

            CardParseResult result = _parser.Parse(cardsAsText, DeckTextParseOptions.WithSideBoard);

            Assert.That(result.Sideboard.Count, Is.EqualTo(5));
            Assert.That(result.Sideboard.Sum(item => item.Count), Is.EqualTo(15));
        }

        [FsCheck.NUnit.Property]
        public void CleanUpCardsQuickCheck(string input)
        {        
            //Act
            _parser.Parse(input, DeckTextParseOptions.WithSideBoard);
        }

        [FsCheck.NUnit.Property]
        public void CleanUpCardsQuickCheckLineInput(int numberOfCards, string cardName)
        {
            //Arrange            
            string textDeck = $"{numberOfCards} { cardName}";

            //Act
            _parser.Parse(textDeck, DeckTextParseOptions.WithSideBoard);
        }

        [FsCheck.NUnit.Property]
        public void CleanUpCardsQuickCheckSuccess(NonZeroInt numberOfCards, NonWhiteSpaceString cardName)
        {
            //Arrange
            var cardNameNoLineBreaks = cardName.Get.Replace("\n", "");
            string textDeck = $"{numberOfCards.Get} {cardNameNoLineBreaks}";

            //Act
            var parseResult = _parser.Parse(textDeck);
            
            //Assert
            parseResult.ResultCode.Should().Be(ParseResultCode.Success, parseResult.Message);
            Assert.That(parseResult.Cards[0].Count ,Is.EqualTo(numberOfCards.Get));
            Assert.That(parseResult.Cards[0].Name, Is.EqualTo(cardNameNoLineBreaks.Trim()));
        }

        [Test]
        [TestCase(1, "a")]
        [TestCase(1,"\u0011")]
        public void CleanUpCardsChecks(int numberOfCards, string cardName)
        {
            string textDeck = $"{numberOfCards} {cardName}";

            //Act
            var parseResult = _parser.Parse(textDeck);

            //Assert
            Assert.That(parseResult.Cards[0].Count ,Is.EqualTo(numberOfCards));
            Assert.That(parseResult.Cards[0].Name, Is.EqualTo(cardName));
        }
    }
}