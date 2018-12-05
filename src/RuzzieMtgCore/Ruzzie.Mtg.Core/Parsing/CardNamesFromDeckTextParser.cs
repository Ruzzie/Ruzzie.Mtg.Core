using System;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public class CardNamesFromDeckTextParser : ICardNamesFromDeckTextParser
    {
        public CardParseResult Parse(string cardsText, DeckTextParseOptions deckTextParseOptions = DeckTextParseOptions.SkipSideboard)
        {
            if (string.IsNullOrWhiteSpace(cardsText))
            {
                return new CardParseResult
                    {Message = "input text is null or empty.", ResultCode = ParseResultCode.NotParsedEmptyText};
            }

            string[] lines = cardsText.Trim().Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);

            return ParseAllLines(deckTextParseOptions, lines);
        }

        private CardParseResult ParseAllLines(DeckTextParseOptions deckTextParseOptions, in string[] lines)
        {
            CardParseResult result = new CardParseResult();

            bool inSideboardLines = false;
            for (var i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                var trimmedLine = line.Trim();

                bool sideBoardStart = StringComparer.OrdinalIgnoreCase.Equals("sideboard", trimmedLine);
                bool skipLine = false;

                if (deckTextParseOptions == DeckTextParseOptions.SkipSideboard && sideBoardStart)
                {
                    //Sideboard reached return
                    break;
                }

                if (deckTextParseOptions == DeckTextParseOptions.WithSideBoard && sideBoardStart)
                {
                    inSideboardLines = true;
                    skipLine = true;
                }

                if (skipLine)
                {
                    continue;
                }

                CardNameAndCount cardCountAndCard = ParseLine(trimmedLine);

                if (cardCountAndCard.Name == null)
                {
                    result.ResultCode = ParseResultCode.Failed;
                    result.Message += $"Line \"{line}\" is not valid. Please check your spelling. ";
                }
                else
                {
                    result.ResultCode = ParseResultCode.Success;
                    if (inSideboardLines)
                    {
                        result.Sideboard.Add(cardCountAndCard);
                    }
                    else
                    {
                        result.Cards.Add(cardCountAndCard);
                    }
                }
            }

            return result;
        }


        private static CardNameAndCount ParseLine(in string line)
        {
            //a line looks like:[number][space][text]

            //expects a trimmed line
            int count = 0;
            string card = null;

            //split on all whitespace characters            
            string[] splittedLineByAllWhitespaces = line.Split(null);

            if (splittedLineByAllWhitespaces.Length > 0)
            {
                //The first element should be a number, try to parse it as a number
                if (int.TryParse(splittedLineByAllWhitespaces[0], out count))
                {                    
                    //trim the part without the number, that should be the cardname on the line     
                    if (line.Length > (splittedLineByAllWhitespaces[0].Length + 1))//when line does not solely contain number
                    {                        
                        string cardName = line.Substring(splittedLineByAllWhitespaces[0].Length + 1).Trim();

                        //not an empty cardname
                        if (!string.IsNullOrWhiteSpace(cardName))
                        {
                            card = cardName;
                        }
                        else
                        {
                            count = 0;
                        }
                    }                               
                }
            }                           
            return new CardNameAndCount(card, count);
        }
    }
}