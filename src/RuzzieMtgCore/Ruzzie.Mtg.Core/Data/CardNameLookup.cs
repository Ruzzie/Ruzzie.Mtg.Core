using System;
using System.Collections.Generic;
using System.Linq;

namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Can be used to lookup types by name. This is meant for mtg cards and has specific logic trying to match cardnames for that purpose.
    /// </summary>
    /// <typeparam name="TCard">The type of the card.</typeparam>
    /// <seealso cref="ICardNameLookup{TCard}" />
    public class CardNameLookup<TCard> : ICardNameLookup<TCard> where TCard : IHasName
    {
        private readonly IQueryable<TCard> _allCards;
        private readonly IEqualityComparer<TCard> _comparer;
        private static readonly TCard Empty = default(TCard);

        /// <summary>
        /// Initializes a new instance of the <see cref="CardNameLookup{TCard}"/> class.
        /// </summary>
        /// <param name="allCards">All cards.</param>
        public CardNameLookup(IQueryable<TCard> allCards) : this(allCards, EqualityComparer<TCard>.Default)
        {           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardNameLookup{TCard}"/> class.
        /// </summary>
        /// <param name="allCards">All cards.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CardNameLookup(IQueryable<TCard> allCards, IEqualityComparer<TCard> comparer)
        {
            if (allCards == null)
            {
                throw new ArgumentNullException(nameof(allCards));
            }
            _allCards = allCards;
            _comparer = comparer;
        }
        
        /// <summary>
        /// Finds object matching the (casinsensitive) name. If an empty string is passed (also after a trim). The default(TCard) is returned.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <returns>The lookup result.</returns>
        public INameLookupResult<TCard> FindCardByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {                
                return new LookupResult<TCard> { MatchResult = LookupMatchResult.NoMatch };
            }

            name = name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return new LookupResult<TCard> { MatchResult = LookupMatchResult.NoMatch };
            }

            return LookupCardByName(name);
        }

        private INameLookupResult<TCard> LookupCardByName(string cardname)
        {
            TCard listedCard = FindCardByExactName(cardname);

            if (_comparer.Equals(listedCard, Empty))
            {
                listedCard = Search(cardname);
            }

            var lookupResult = new LookupResult<TCard>();
            lookupResult.ResultObject = listedCard;

            if (_comparer.Equals(listedCard, Empty))
            {
                lookupResult.MatchResult = LookupMatchResult.NoMatch;
            }
            else
            {
                lookupResult.MatchResult = LookupMatchResult.Match;
            }
            return lookupResult;
        }

        private class LookupResult<T> : INameLookupResult<T>
        {
            public T ResultObject { get; set; }
            public LookupMatchResult MatchResult { get; set; }
        }

        private TCard Search(string cardname)
        {
            TCard listedCard = FindCardByExactName(cardname.Replace(" s ", "\'s "));

            if ( _comparer.Equals(listedCard,Empty))
            {
                string synonym = GetSynonym(cardname);
                if (synonym != null)
                {
                    listedCard = FindCardByExactName(synonym);
                }
            }

            cardname = cardname.Replace(" s ", "\'s ");

            if ( _comparer.Equals(listedCard,Empty))
            {
                var cardWithCommaAfterFirstWord = AddCommaAfterFirstWordIfNoCommaPresent(cardname);
                if (cardWithCommaAfterFirstWord != cardname)
                {
                    listedCard = FindCardByExactName(cardWithCommaAfterFirstWord);
                }
            }

            if ( _comparer.Equals(listedCard,Empty))
            {
                var cardWithCommaAfterSecondWord = AddCommaAfterSecondWordIfNoCommaPresent(cardname);
                if (cardWithCommaAfterSecondWord != cardname)
                {
                    listedCard = FindCardByExactName(cardWithCommaAfterSecondWord);
                }
            }

            if ( _comparer.Equals(listedCard,Empty))
            {
                var cardWithCommaAfterThirdWord = AddCommaAfterThirdWordIfNoCommaPresent(cardname);
                if (cardWithCommaAfterThirdWord != cardname)
                {
                    listedCard = FindCardByExactName(cardWithCommaAfterThirdWord);
                }
            }

            if ( _comparer.Equals(listedCard,Empty) && cardname.Contains("ther"))
            {
                listedCard = FindCardByExactName(cardname.Replace("ther", "Aether"));
                if ( _comparer.Equals(listedCard,Empty))
                {
                    listedCard = FindCardByExactName(cardname.Replace("ther", "Æther"));
                }
            }


            if ( _comparer.Equals(listedCard,Empty))
            {
                var words = cardname.Split(new[] { " " }, StringSplitOptions.None);
                listedCard = TryFindCardByVariatingWordJoiningOptions(words);
                if ( _comparer.Equals(listedCard,Empty))
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        if ( _comparer.Equals(listedCard,Empty) && words[i].EndsWith("s", StringComparison.OrdinalIgnoreCase))
                        {
                            words[i] = words[i] + "\'";
                            string cardWithApostrophesAddedAfterWord = string.Join(" ", words);
                            listedCard = FindCardByExactName(cardWithApostrophesAddedAfterWord);

                            if ( _comparer.Equals(listedCard,Empty))
                            {
                                listedCard = TryFindCardByVariatingWordJoiningOptions(words);
                            }
                        }
                    }
                }

                words = cardname.Split(new[] { " " }, StringSplitOptions.None);
                if ( _comparer.Equals(listedCard,Empty) && words[0].EndsWith("'s", StringComparison.OrdinalIgnoreCase))
                {
                    words[0] = words[0].Replace("\'s", "s\'");
                    string cardWithApostropheAddedAfterfirstWord = string.Join(" ", words);
                    listedCard = FindCardByExactName(cardWithApostropheAddedAfterfirstWord);

                    if ( _comparer.Equals(listedCard,Empty))
                    {
                        listedCard = TryFindCardByVariatingWordJoiningOptions(words);
                    }
                }
            }
            return listedCard;
        }

        private TCard TryFindCardByVariatingWordJoiningOptions(string[] words)
        {
            TCard listedCard = Empty;
            if (words.Length == 2)
            {
                //check if dual card
                listedCard = FindCardByExactName(string.Join(" // ", words));

                if ( _comparer.Equals(listedCard,Empty))
                {
                    listedCard = FindCardByExactName(string.Join("-", words));
                }
            }

            if ( _comparer.Equals(listedCard,Empty) && words.Length == 3)
            {
                listedCard = VariateWordsForCardnameWithLengthThree(words);
            }

            if ( _comparer.Equals(listedCard,Empty) && words.Length == 4)
            {
                listedCard = VariateWordsForCardnameWithLengthFour(words);
            }


            if ( _comparer.Equals(listedCard,Empty) && words.Length > 2)
            {
                //hyphen all words
                listedCard = FindCardByExactName(string.Join("-", words));
            }


            if ( _comparer.Equals(listedCard,Empty) && words.Length > 4)
            {
                listedCard = VariateWordsForCardnameWithLengthGreaterThanFour(words);
            }

            if ( _comparer.Equals(listedCard,Empty))
            {
                //hyphen first and last two words
                string[] firstTwoWordsHyphened = CombineNextTwoWords(0, words, "-");

                listedCard = VariateWordsForCardnameWithLengthGreaterThanFour(firstTwoWordsHyphened);
            }

            return listedCard;
        }

        private TCard VariateWordsForCardnameWithLengthFour(string[] words)
        {
            TCard listedCard;
            //check if - is forgotten between words
            //first the most left two words then the right two words
            var leftHyphen = words[0] + "-" + words[1] + " " + words[2] + " " + words[3];
            var rightHyphen = words[0] + " " + words[1] + " " + words[2] + "-" + words[3];
            var middleHyphen = words[0] + " " + words[1] + "-" + words[2] + " " + words[3];

            TCard firstOption = FindCardByExactName(leftHyphen);
            //Card secondOption = FindCard(rightHyphen);
            //Card thirdOption = FindCard(middleHyphen);

            //TODO: Check if optimization is possible, bu not calling all findcards before comparing
            listedCard = ! _comparer.Equals(firstOption,Empty)? firstOption : FindCardByExactName(rightHyphen); //(secondOption == C)

            if ( _comparer.Equals(listedCard,Empty))
            {
                listedCard = FindCardByExactName(middleHyphen);
            }

            //listedCard = firstOption ?? secondOption ?? thirdOption;


            if ( _comparer.Equals(listedCard,Empty))
            {
                //try with , after first word
                var leftHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(leftHyphen);
                var rightHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(rightHyphen);
                var middleHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(middleHyphen);
                if (leftHyphenWithComma != leftHyphen)
                {
                    listedCard = FindCardByExactName(leftHyphenWithComma);
                }

                if ( _comparer.Equals(listedCard,Empty) && rightHyphenWithComma != rightHyphen)
                {
                    listedCard = FindCardByExactName(rightHyphenWithComma);
                }

                if ( _comparer.Equals(listedCard,Empty) && middleHyphenWithComma != middleHyphen)
                {
                    listedCard = FindCardByExactName(middleHyphenWithComma);
                }
            }

            return listedCard;
        }

        private TCard VariateWordsForCardnameWithLengthGreaterThanFour(string[] words)
        {
            TCard listedCard;

            for (int i = 0; i < words.Length; i++)
            {
                string[] theWords = CombineNextTwoWords(i, words, "-");
                string hypenedCardname = string.Join(" ", theWords);
                listedCard = FindCardByExactName(hypenedCardname);

                if ( _comparer.Equals(listedCard,Empty))
                {
                    //try with comma after each word
                    for (int j = 0; j < theWords.Length - 1; j++)
                    {
                        string[] tmpCardNameWithCommaAfterWord = new string[theWords.Length];
                        theWords.CopyTo(tmpCardNameWithCommaAfterWord, 0);
                        tmpCardNameWithCommaAfterWord[j] = tmpCardNameWithCommaAfterWord[j] + ",";

                        listedCard = FindCardByExactName(string.Join(" ", tmpCardNameWithCommaAfterWord));

                        if (! _comparer.Equals(listedCard,Empty))
                        {
                            return listedCard;
                        }
                    }
                }
                else
                {
                    return listedCard;

                }
            }

            return Empty;
        }

        private static string[] CombineNextTwoWords(int startWordIndex, string[] words, string combinator = " ")
        {
            List<string> combined = new List<string>();
            for (int i = 0; i < words.Length; i++)
            {
                if (i == startWordIndex)
                {
                    if (i + 1 < words.Length)
                    {
                        combined.Add(words[i] + combinator + words[i + 1]);
                    }
                }
                else
                {
                    if (i != startWordIndex + 1)
                    {
                        combined.Add(words[i]);
                    }
                }
            }
            return combined.ToArray();
        }

        private TCard VariateWordsForCardnameWithLengthThree(string[] words)
        {
            //check if - is forgotten between words
            //first the most left two words then the right two words
            var leftHyphen = words[0] + "-" + words[1] + " " + words[2];
            var rightHyphen = words[0] + " " + words[1] + "-" + words[2];

            TCard firstOption = FindCardByExactName(leftHyphen);
            TCard listedCard = ! _comparer.Equals(firstOption,Empty) ? firstOption : FindCardByExactName(rightHyphen);
            //Card listedCard = firstOption ??
            //                  secondOption;
            if ( _comparer.Equals(listedCard,Empty))
            {
                //try with , after first word
                var leftHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(leftHyphen);
                var rightHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(rightHyphen);
                if (leftHyphenWithComma != leftHyphen)
                {
                    listedCard = FindCardByExactName(leftHyphenWithComma);
                }

                if ( _comparer.Equals(listedCard,Empty) && rightHyphenWithComma != rightHyphen)
                {
                    listedCard = FindCardByExactName(rightHyphenWithComma);
                }
            }
            return listedCard;
        }

        private static string AddCommaAfterSecondWordIfNoCommaPresent(string cardname)
        {
            var words = cardname.Split(new[] { " " }, StringSplitOptions.None);
            if (words.Length >= 3 && !words[0].EndsWith(","))
            {
                words[1] = words[1] + ",";
                var cardnameWithCommaAfterFirstWord = string.Join(" ", words);
                return cardnameWithCommaAfterFirstWord;
            }

            return cardname;
        }

        private static string AddCommaAfterThirdWordIfNoCommaPresent(string cardname)
        {
            var words = cardname.Split(new[] { " " }, StringSplitOptions.None);
            if (words.Length >= 4 && !words[0].EndsWith(","))
            {
                words[2] = words[2] + ",";
                var cardnameWithCommaAfterFirstWord = string.Join(" ", words);
                return cardnameWithCommaAfterFirstWord;
            }

            return cardname;
        }

        private static string AddCommaAfterFirstWordIfNoCommaPresent(string cardname)
        {
            var words = cardname.Split(new[] { " " }, StringSplitOptions.None);
            if (words.Length > 1 && !words[0].EndsWith(","))
            {
                words[0] = words[0] + ",";
                var cardnameWithCommaAfterFirstWord = string.Join(" ", words);
                return cardnameWithCommaAfterFirstWord;
            }

            return cardname;
        }

        private static string GetSynonym(string cardname)
        {
            cardname = cardname.ToLowerInvariant();

            if (cardname == "lim d l s vault")
            {
                return "Lim-Dûl's Vault";
            }

            if (cardname == "ther vial")
            {
                return "Æther Vial";
            }

            if (cardname == "man o war")
            {
                return "Man-o'-War";
            }

            if (cardname == "thersnipe")
            {
                return "Æthersnipe";
            }

            if (cardname == "gods eye gate to the reikai")
            {
                return "Gods' Eye, Gate to the Reikai";
            }

            if (cardname == "j tun grunt")
            {
                return "Jötun Grunt";
            }

            if (cardname == "silvergill adep")
            {
                return "Silvergill adept";
            }         

            if (cardname.Contains("rune of protection "))
            {
                return cardname.Replace("protection ", "protection: ");
            }

            if (cardname.Contains("circle of protection "))
            {
                return cardname.Replace("protection ", "protection: ");
            }

            if (cardname.Contains("s ance"))
            {
                return cardname.Replace("s ance", "Séance");
            }

            if (cardname.Contains("fa adiyah seer"))
            {
                return cardname.Replace("fa adiyah seer", "Fa'adiyah Seer");
            }
          
            if (cardname.Contains("will o the wisp"))
            {
                return cardname.Replace("will o the wisp", "Will-o'-the-Wisp");
            }

            if (cardname == "snapc")
            {
                return "Snapcaster Mage";
            }

            return null;
        }

        private TCard FindCardByExactName(string cardname)
        {
            TCard firstOrDefault = _allCards
                .FirstOrDefault(card => string.Equals(card.Name, cardname, StringComparison.InvariantCultureIgnoreCase));

            return firstOrDefault;
        }
    }
}