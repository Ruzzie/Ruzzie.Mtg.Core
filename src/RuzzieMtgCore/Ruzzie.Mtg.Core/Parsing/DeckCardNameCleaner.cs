using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public class DeckCardNameCleaner<TCard> : IDeckCardNameCleaner<TCard> where TCard:IBasicCardProperties
    {
        private readonly ICardNameLookup<TCard> _cardNameLookup;

        public DeckCardNameCleaner(ICardNameLookup<TCard> cardNameLookup)
        {
            if (cardNameLookup == null)
            {
                throw new ArgumentNullException(nameof(cardNameLookup));
            }
            _cardNameLookup = cardNameLookup;
        }

        public List<CardNameAndCount> CleanUpCards(List<CardNameAndCount> cardList)
        {
            if (cardList == null)
            {
                throw new ArgumentNullException(nameof(cardList));
            }

            var nameObjectDictionary = CleanCardsForListAndReturnDeckCardDictionaryByCardName(cardList);
            return nameObjectDictionary.Values.Select(x => new CardNameAndCount(x.Card.Name, x.Count)).ToList();
        }

        private ConcurrentDictionary<string, DeckCard<TCard>> CleanCardsForListAndReturnDeckCardDictionaryByCardName(List<CardNameAndCount> cardList)
        {
            var nameObjectDictionary = new ConcurrentDictionary<string, DeckCard<TCard>>(StringComparer.OrdinalIgnoreCase);

            int cardListCount = cardList.Count;

            for (var i = 0; i < cardListCount; i++)
            {
                CardNameAndCount input = cardList[i];

                string currentCardName = input.Name;

                if (string.Equals(currentCardName, "unknown card", StringComparison.OrdinalIgnoreCase) || currentCardName.Length <= 1)
                {
                    continue;
                }

                var cardEntry = CleanCardEntry(input);

                nameObjectDictionary.AddOrUpdate(cardEntry.Card.Name, new DeckCard<TCard>(cardEntry.Card, cardEntry.Count), (key, val) =>
                {
                    val.Count += cardEntry.Count;
                    return val;
                });
            }
            return nameObjectDictionary;
        }

        private DeckCard<TCard> CleanCardEntry(CardNameAndCount cardNameAndCount)
        {
            var lookupResult = _cardNameLookup.FindCardByName(cardNameAndCount.Name);

            var newCount = cardNameAndCount.Count;
            var currentCard = lookupResult.ResultObject;
            var currentCardCount = cardNameAndCount.Count;
            var currentCardName = cardNameAndCount.Name;

            switch (lookupResult.MatchResult)
            {
                case LookupMatchResult.FuzzyMatch:
                case LookupMatchResult.Match:
                    
                    if (!((BasicType) lookupResult.ResultObject.BasicType).ContainsAnyBasicType(BasicType.BasicLand)
                        && (currentCardCount == 0 || currentCardCount > 4))
                    {
                        if (currentCardCount > 4 && currentCardCount % 4 == 0)
                        {
                            newCount = 4;
                        }
                        else
                        {
                            //Often there are typos, like 25 Island Sanctuary when 25 Island the actual intent
                            //So try to see if a cardname contains the words of a basic land
                            var matchedBasicLandNameResult = CardNameContainsBasicLandTypeName(currentCardName);
                            if (matchedBasicLandNameResult.MatchResult == LookupMatchResult.Match)
                            {
                                //if it does, change the name to the found basic type
                                currentCard = matchedBasicLandNameResult.ResultObject;
                            }
                            else
                            {
                                if (currentCardCount / 2 < 8)
                                {
                                    //this indicates that somebody probably entered the same line twice, ex.: 3 and 3, is 6.
                                    //input.Count = currentCardCount / 2;
                                    newCount = currentCardCount / 2;
                                }
                                else
                                {
                                    throw new ArgumentException($"Card with name {currentCardName} has an invalid cardcount of {currentCardCount}.");
                                }
                            }
                        }
                    }
                    break;
                case LookupMatchResult.NoMatch:
                    throw new ArgumentException($"Card with name {currentCardName} was not found.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new DeckCard<TCard>(currentCard, newCount);
        }

        public DeckCards<TCard> CleanUpCards(List<CardNameAndCount> mainboard, List<CardNameAndCount> sideboard)
        {
            if (mainboard == null)
            {
                throw new ArgumentNullException(nameof(mainboard));
            }
            if (sideboard == null)
            {
                throw new ArgumentNullException(nameof(sideboard));
            }
            var cleanedMainboard = CleanCardsForListAndReturnDeckCardDictionaryByCardName(mainboard);
            var cleanedSideBoard = CleanCardsForListAndReturnDeckCardDictionaryByCardName(sideboard);
            return new DeckCards<TCard> {Mainboard = cleanedMainboard.Values.ToList(), Sideboard = cleanedSideBoard.Values.ToList() };
        }

        // ReSharper disable StaticMemberInGenericType
        private static readonly string[] CardWordSeparators = {" ","-",",","\t"};               
        // ReSharper restore StaticMemberInGenericType

        private INameLookupResult<TCard> CardNameContainsBasicLandTypeName(string cardName)
        {
            var allWords = cardName.Split(CardWordSeparators, StringSplitOptions.RemoveEmptyEntries);
          
            string found = allWords.FirstOrDefault(word => BasicTypes.BasicLandCardNames.Contains(word));

            if (found != null)
            {
               return _cardNameLookup.FindCardByName(found);                
            }
            var result = new NameLookupResult<TCard>();
            return result;
        }
    }
}