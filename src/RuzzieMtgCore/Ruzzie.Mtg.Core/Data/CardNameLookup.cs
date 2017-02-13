using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ruzzie.Common.Hashing;
using Ruzzie.FuzzyStrings;

namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Can be used to lookup types by partialName. This is meant for mtg cards and has specific logic trying to match cardnames for that purpose.
    /// </summary>
    /// <typeparam name="TCard">The type of the card.</typeparam>
    /// <seealso cref="ICardNameLookup{TCard}" />
    public class CardNameLookup<TCard> : ICardNameLookup<TCard> where TCard : IHasName
    {
        private readonly IEqualityComparer<TCard> _comparer;
        private readonly double _minProbability;
        private static readonly TCard Empty = default(TCard);

        private readonly ICardNameLookupRepository<TCard> _lookupRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardNameLookup{TCard}"/> class.
        /// </summary>
        /// <param name="allCards">All cards.</param>
        public CardNameLookup(IQueryable<TCard> allCards) : this(allCards, EqualityComparer<TCard>.Default)
        {           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardNameLookup{TCard}" /> class.
        /// </summary>
        /// <param name="typeWithAllCards">The type with all cards.</param>
        public CardNameLookup(IHasQuerableAllCards<TCard> typeWithAllCards): this(typeWithAllCards.AllCards)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardNameLookup{TCard}" /> class.
        /// </summary>
        /// <param name="typeWithAllCards">The type with all cards.</param>
        /// <param name="comparer">The comparer to use to compare whith default (notfound) value. This is not used for cardname comparisons.</param>
        public CardNameLookup(IHasQuerableAllCards<TCard> typeWithAllCards, IEqualityComparer<TCard> comparer) : this(typeWithAllCards.AllCards, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardNameLookup{TCard}"/> class.
        /// </summary>
        /// <param name="allCards">All cards.</param>
        /// <param name="comparer">The comparer to use to compare whith default (notfound) value. This is not used for cardname comparisons.</param>
        /// <param name="minProbability">The minimum probability threshold for fuzzy matching. Should be between ~0.7 and 0.99</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CardNameLookup(IQueryable<TCard> allCards, IEqualityComparer<TCard> comparer, double minProbability = 0.689D)
        {
            if (allCards == null)
            {
                throw new ArgumentNullException(nameof(allCards));
            }

            _lookupRepository = new CardNameLookupRepository<TCard>(allCards);

            _comparer = comparer;
            _minProbability = minProbability;
        }

        /// <summary>
        /// Returns all probable matches for a partial name.
        /// </summary>
        /// <param name="partialName">The partialName.</param>
        /// <param name="minProbability"></param>
        /// <param name="maxresults">The maxresults.</param>
        /// <returns>
        /// the lookupresults.
        /// </returns>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// minprobability must be > 0
        /// maxresults must be > 0
        /// </exception>
        public IEnumerable<INameLookupResult<TCard>> LookupCardName(string partialName, double minProbability, int maxresults = 10)
        {
            if (string.IsNullOrWhiteSpace(partialName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(partialName));
            }
            if (minProbability <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minProbability));
            }

            if (maxresults <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxresults));
            }

            //get all fuzzy matches
            var allFuzzyMatches = _lookupRepository.FindAllFuzzyMatchesAboveThreshold(partialName, minProbability, maxresults);

            return
                allFuzzyMatches.Select(
                    match =>
                        new NameLookupResult<TCard>
                        {
                            MatchResult = LookupMatchResult.FuzzyMatch,
                            ResultObject = match.Value,
                            MatchProbability = match.MatchProbability
                        });
        }

        /// <summary>
        /// Finds object matching the (casinsensitive) partialName. If an empty string is passed (also after a trim). The default(TCard) is returned.
        /// </summary>
        /// <param name="name">The name of the card to find.</param>
        /// <returns>The lookup result.</returns>
        public INameLookupResult<TCard> FindCardByName(string name)
        {
           return FindCardByName(name, _minProbability);
        }

        /// <summary>
        /// Finds object matching the (casinsensitive) partialName. If an empty string is passed (also after a trim). The default(TCard) is returned.
        /// </summary>
        /// <param name="name">The name of the card to find.</param>
        /// <param name="minProbability">The minimum probability threshold for fuzzy matching. Should be between 0.75 and 0.99</param>
        /// <returns>The lookup result.</returns>
        public INameLookupResult<TCard> FindCardByName(string name, double minProbability)
        {
            if (string.IsNullOrWhiteSpace(name))
            {                
                return new NameLookupResult<TCard> { MatchResult = LookupMatchResult.NoMatch };
            }

            name = name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return new NameLookupResult<TCard> { MatchResult = LookupMatchResult.NoMatch };
            }

            return LookupSingleCardByName(name, minProbability);
        }

        private INameLookupResult<TCard> LookupSingleCardByName(string cardname, double minProbability)
        {          
            TCard listedCard = _lookupRepository.FindCardByExactName(cardname);

            if (_comparer.Equals(listedCard, Empty))
            {
                return Search(cardname, minProbability);
            }
            else
            {
                var lookupResult = new NameLookupResult<TCard>();
                lookupResult.MatchResult = LookupMatchResult.Match;
                lookupResult.ResultObject = listedCard;
                lookupResult.MatchProbability = 1.0;
                return lookupResult;
            }
        }

        private INameLookupResult<TCard> Search(string cardname, double minProbability)
        {
            var lookupResult = new NameLookupResult<TCard>();

            TCard listedCard = _lookupRepository.FindCardByExactName(cardname.Replace(" s ", "\'s ").Replace(" / "," // "));
           
            if ( _comparer.Equals(listedCard,Empty))
            {
                string synonym = KnownSynonyms.GetSynonym(cardname);
                if (synonym != null)
                {
                    listedCard = _lookupRepository.FindCardByExactName(synonym);
                }
            }
                    
            cardname = cardname.Replace(" s ", "\'s ");

            if ( _comparer.Equals(listedCard,Empty))
            {
                var cardWithCommaAfterFirstWord = AddCommaAfterFirstWordIfNoCommaPresent(cardname);
                if (cardWithCommaAfterFirstWord != cardname)
                {
                    listedCard = _lookupRepository.FindCardByExactName(cardWithCommaAfterFirstWord);
                }
            }

            if ( _comparer.Equals(listedCard,Empty))
            {
                var cardWithCommaAfterSecondWord = AddCommaAfterSecondWordIfNoCommaPresent(cardname);
                if (cardWithCommaAfterSecondWord != cardname)
                {
                    listedCard = _lookupRepository.FindCardByExactName(cardWithCommaAfterSecondWord);
                }
            }

            if ( _comparer.Equals(listedCard,Empty))
            {
                var cardWithCommaAfterThirdWord = AddCommaAfterThirdWordIfNoCommaPresent(cardname);
                if (cardWithCommaAfterThirdWord != cardname)
                {
                    listedCard = _lookupRepository.FindCardByExactName(cardWithCommaAfterThirdWord);
                }
            }

            if ( _comparer.Equals(listedCard,Empty) && cardname.Contains("ther"))
            {
                listedCard = _lookupRepository.FindCardByExactName(cardname.Replace("ther", "Aether"));
                if ( _comparer.Equals(listedCard,Empty))
                {
                    listedCard = _lookupRepository.FindCardByExactName(cardname.Replace("ther", "Æther"));
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
                        var wordEndsWith = "s";
                        var stringToAddAfterWord = "\'";
                        listedCard = TryFindByVariatingCharacterOnEndOfWord(listedCard, words, i, wordEndsWith, stringToAddAfterWord);
                    }
                }

                words = cardname.Split(new[] { " " }, StringSplitOptions.None);
                if ( _comparer.Equals(listedCard,Empty) && words[0].EndsWith("'s", StringComparison.OrdinalIgnoreCase))
                {
                    words[0] = words[0].Replace("\'s", "s\'");
                    string cardWithApostropheAddedAfterfirstWord = string.Join(" ", words);
                    listedCard = _lookupRepository.FindCardByExactName(cardWithApostropheAddedAfterfirstWord);

                    if ( _comparer.Equals(listedCard,Empty))
                    {
                        listedCard = TryFindCardByVariatingWordJoiningOptions(words);
                    }
                }
            }

            if (_comparer.Equals(listedCard, Empty))
            {
                var words = cardname.Split(new[] {" "}, StringSplitOptions.None);
                listedCard = TryFindCardByVariatingWordJoiningOptions(words);
                if (_comparer.Equals(listedCard, Empty))
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        var wordEndsWith = "s";
                        var stringToAddAfterWord = "\'s";
                        listedCard = TryFindByVariatingCharacterOnEndOfWord(listedCard, words, i, wordEndsWith, stringToAddAfterWord);
                    }
                }
            }

            if (_comparer.Equals(listedCard, Empty))
            {
                var words = cardname.Split(new[] { " " }, StringSplitOptions.None);
                listedCard = TryFindCardByVariatingWordJoiningOptions(words);
                if (_comparer.Equals(listedCard, Empty))
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i].EndsWith("s") && !words[i].EndsWith("'s"))
                        {
                            words[i] = words[i].Replace("s", "");
                            string cardWithWordThatEndsInSRemovedS = string.Join(" ", words);
                            listedCard = _lookupRepository.FindCardByExactName(cardWithWordThatEndsInSRemovedS);

                            if (_comparer.Equals(listedCard, Empty))
                            {
                                listedCard = TryFindCardByVariatingWordJoiningOptions(words);
                            }
                        }
                    }
                }

            }

            //Lastly do a full fuzzy match if nothing is found so far
            if (_comparer.Equals(listedCard, Empty))
            {
                FuzzyMatch<TCard> fuzzyMatch = _lookupRepository.FindCardByFuzzyMatch(cardname, minProbability);
                listedCard = fuzzyMatch.Value;

                if (!_comparer.Equals(listedCard, Empty))
                {
                    //now we have a fuzzymatch
                    lookupResult.MatchResult = LookupMatchResult.FuzzyMatch;
                    lookupResult.MatchProbability = fuzzyMatch.MatchProbability;
                }
            }
            else
            {
                lookupResult.MatchProbability = 1.0;
                lookupResult.MatchResult = LookupMatchResult.Match;
            }

            lookupResult.ResultObject = listedCard;
            return lookupResult;
        }

        private TCard TryFindByVariatingCharacterOnEndOfWord(TCard listedCard, string[] words, int i, string wordEndsWith, string afterWordWithCharacherString)
        {
            if (_comparer.Equals(listedCard, Empty) && words[i].EndsWith(wordEndsWith, StringComparison.OrdinalIgnoreCase))
            {
                words[i] = words[i] + afterWordWithCharacherString;
                string cardWithApostrophesAddedAfterWord = string.Join(" ", words);
                listedCard = _lookupRepository.FindCardByExactName(cardWithApostrophesAddedAfterWord);

                if (_comparer.Equals(listedCard, Empty))
                {
                    listedCard = TryFindCardByVariatingWordJoiningOptions(words);
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
                listedCard = _lookupRepository.FindCardByExactName(string.Join(" // ", words));

                if ( _comparer.Equals(listedCard,Empty))
                {
                    listedCard = _lookupRepository.FindCardByExactName(string.Join("-", words));
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
                listedCard = _lookupRepository.FindCardByExactName(string.Join("-", words));
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
            //check if - is forgotten between words
            //first the most left two words then the right two words
            var leftHyphen = words[0] + "-" + words[1] + " " + words[2] + " " + words[3];
            var rightHyphen = words[0] + " " + words[1] + " " + words[2] + "-" + words[3];
            var middleHyphen = words[0] + " " + words[1] + "-" + words[2] + " " + words[3];

            TCard firstOption = _lookupRepository.FindCardByExactName(leftHyphen);

            //TODO: Check if optimization is possible, bu not calling all findcards before comparing
            TCard listedCard = !_comparer.Equals(firstOption, Empty) ? firstOption : _lookupRepository.FindCardByExactName(rightHyphen);

            if ( _comparer.Equals(listedCard,Empty))
            {
                listedCard = _lookupRepository.FindCardByExactName(middleHyphen);
            }

            if ( _comparer.Equals(listedCard,Empty))
            {
                //try with , after first word
                var leftHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(leftHyphen);
                var rightHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(rightHyphen);
                var middleHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(middleHyphen);
                if (leftHyphenWithComma != leftHyphen)
                {
                    listedCard = _lookupRepository.FindCardByExactName(leftHyphenWithComma);
                }

                if ( _comparer.Equals(listedCard,Empty) && rightHyphenWithComma != rightHyphen)
                {
                    listedCard = _lookupRepository.FindCardByExactName(rightHyphenWithComma);
                }

                if ( _comparer.Equals(listedCard,Empty) && middleHyphenWithComma != middleHyphen)
                {
                    listedCard = _lookupRepository.FindCardByExactName(middleHyphenWithComma);
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
                listedCard = _lookupRepository.FindCardByExactName(hypenedCardname);

                if ( _comparer.Equals(listedCard,Empty))
                {
                    //try with comma after each word
                    for (int j = 0; j < theWords.Length - 1; j++)
                    {
                        string[] tmpCardNameWithCommaAfterWord = new string[theWords.Length];
                        theWords.CopyTo(tmpCardNameWithCommaAfterWord, 0);
                        tmpCardNameWithCommaAfterWord[j] = tmpCardNameWithCommaAfterWord[j] + ",";

                        listedCard = _lookupRepository.FindCardByExactName(string.Join(" ", tmpCardNameWithCommaAfterWord));

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

            //There is nothing to combine so return original.
            if (words.Length <= 1 || startWordIndex == words.Length - 1)
            {
                return words;
            }

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

            TCard firstOption = _lookupRepository.FindCardByExactName(leftHyphen);
            TCard listedCard = ! _comparer.Equals(firstOption,Empty) ? firstOption : _lookupRepository.FindCardByExactName(rightHyphen);

            if ( _comparer.Equals(listedCard,Empty))
            {
                //try with , after first word
                var leftHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(leftHyphen);
                var rightHyphenWithComma = AddCommaAfterFirstWordIfNoCommaPresent(rightHyphen);
                if (leftHyphenWithComma != leftHyphen)
                {
                    listedCard = _lookupRepository.FindCardByExactName(leftHyphenWithComma);
                }

                if ( _comparer.Equals(listedCard,Empty) && rightHyphenWithComma != rightHyphen)
                {
                    listedCard = _lookupRepository.FindCardByExactName(rightHyphenWithComma);
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
    }

    /// <summary>
    /// Compares Cards only on name
    /// </summary>
    /// <typeparam name="TCard">The type of the card.</typeparam>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{TCard}" />
    public class CardNameEqualityComparer<TCard> : IEqualityComparer<TCard> where TCard : IHasName
    {
        private readonly StringComparer _nameComparerToUse;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardNameEqualityComparer{TCard}"/> class. Uses OrdinalIgnoreCase by default.
        /// </summary>
        public CardNameEqualityComparer()
        {
            _nameComparerToUse = StringComparer.OrdinalIgnoreCase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardNameEqualityComparer{TCard}"/> class.
        /// </summary>
        /// <param name="nameComparerToUse">The name comparer to use.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CardNameEqualityComparer(StringComparer nameComparerToUse)
        {
            if (nameComparerToUse == null)
            {
                throw new ArgumentNullException(nameof(nameComparerToUse));
            }
            _nameComparerToUse = nameComparerToUse;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="TCard" /> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="TCard" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(TCard x, TCard y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(x, null))
            {
                return false;
            }
            if (ReferenceEquals(y, null))
            {
                return false;
            }
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return _nameComparerToUse.Equals(x.Name, y.Name) ||
                   _nameComparerToUse.Equals(x.Name.CreateValidUpperCaseKeyForString(), y.Name.CreateValidUpperCaseKeyForString());
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(TCard obj)
        {
            return _nameComparerToUse.GetHashCode(obj.Name.CreateValidUpperCaseKeyForString());
        }
    }

    internal class FuzzyMatch<TCard>
    {
        public TCard Value { get; set; }
        public double MatchProbability { get; set; }
    }

    internal class FuzzyMatchEqualityComparer<TCard> : IEqualityComparer<FuzzyMatch<TCard>> where TCard : IHasName
    {
        private readonly CardNameEqualityComparer<TCard> _equalityComparerImplementation = new CardNameEqualityComparer<TCard>(StringComparer.OrdinalIgnoreCase);

        public bool Equals(FuzzyMatch<TCard> x, FuzzyMatch<TCard> y)
        {
            return _equalityComparerImplementation.Equals(x.Value, y.Value);
        }

        public int GetHashCode(FuzzyMatch<TCard> obj)
        {
            return _equalityComparerImplementation.GetHashCode(obj.Value);
        }
    }

    internal interface ICardNameLookupRepository<TCard> where TCard : IHasName
    {
        TCard FindCardByExactName(string cardname);
        FuzzyMatch<TCard> FindCardByFuzzyMatch(string cardname, double minProbability = 0.85D);
        IEnumerable<FuzzyMatch<TCard>> FindAllFuzzyMatchesAboveThreshold(string cardname, double minProbability = 0.85D, int maxResults = 10);
    }

    internal class CardNameLookupRepository<TCard> : ICardNameLookupRepository<TCard> where TCard : IHasName
    {  
        // ReSharper disable once StaticMemberInGenericType
        private static readonly FNV1AHashAlgorithm64 FNV1AHashAlgorithm64 = new FNV1AHashAlgorithm64();

        public CardNameLookupRepository(IQueryable<TCard> allCards)
        {
            NameLookupDataSource = new CardNameLookupDataSource<TCard>(allCards);
        }

        public ICardNameLookupDataSource<TCard> NameLookupDataSource { get; }

        public TCard FindCardByExactName(string cardname)
        {
            if (string.IsNullOrWhiteSpace(cardname))
            {
                return default(TCard);
            }
            cardname = cardname.Trim();
            long cardNameHash = FNV1AHashAlgorithm64.HashStringCaseInsensitive(cardname.CreateValidUpperCaseKeyForString());
            TCard cardResult;
            if (NameLookupDataSource.HashLookup.TryGetValue(cardNameHash, out cardResult))
            {
                return cardResult;
            }
            return default(TCard);
        }

        public FuzzyMatch<TCard> FindCardByFuzzyMatch(string cardname, double minProbability = 0.689D)
        {
            if (string.IsNullOrWhiteSpace(cardname))
            {
                return new FuzzyMatch<TCard>();
            }

            cardname = cardname.Trim();
            var itemQuery = FindAllFuzzyMatchesAboveThreshold(cardname, minProbability);
            var res = itemQuery.FirstOrDefault();

            if (res != null)
            {
                return new FuzzyMatch<TCard> {Value = res.Value, MatchProbability = res.MatchProbability};
            }
            return new FuzzyMatch<TCard>();
        }

        public IEnumerable<FuzzyMatch<TCard>> FindAllFuzzyMatchesAboveThreshold(string cardname, double minProbability = 0.689D, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(cardname))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(cardname));
            }

            var result = new List<FuzzyMatch<TCard>>();
            cardname = cardname.Trim().RemoveSpecialCharacters();

            var containsMatch =
                NameLookupDataSource.AllDistinctCards
                    .Where(item => item.Name.RemoveSpecialCharacters().ToUpperInvariant().Contains(cardname.ToUpperInvariant()))
                    .Take(maxResults)
                    .Select(item => new FuzzyMatch<TCard> { MatchProbability = CalculateProbalityBasedOnContainsMatch(cardname, item), Value = item }).ToList();

            if (containsMatch.Count >= maxResults)
            {                
                return containsMatch;
            }

            var fuzzyMatchTokensResults = NameLookupDataSource.AllDistinctCards
             .Select(
                 card =>
                     new FuzzyMatch<TCard>
                     {
                         MatchProbability = card.Name.RemoveSpecialCharacters().FuzzyMatchTokens(cardname, false),
                         Value = card
                     })
             .Where(arg => arg.MatchProbability >= minProbability)
             .Take(maxResults)
             .OrderByDescending(arg => arg.MatchProbability).ToList();

            if (fuzzyMatchTokensResults.Count + containsMatch.Count >= maxResults)
            {
                result.AddRange(fuzzyMatchTokensResults);//order of adding seems to matter
                result.AddRange(containsMatch);
                return result;
            }

            var fuzzyMatchResults = NameLookupDataSource.AllDistinctCards
                .Select(
                    card =>
                        new FuzzyMatch<TCard>
                        {
                            MatchProbability = card.Name.RemoveSpecialCharacters().FuzzyMatch(cardname, false),
                            Value = card
                        })
                .Where(arg => arg.MatchProbability >= minProbability)
                .Take(maxResults)
                .OrderByDescending(arg => arg.MatchProbability);
           
            result.AddRange(fuzzyMatchTokensResults);//order of adding seems to matter
            result.AddRange(fuzzyMatchResults);//order of adding seems to matter
            result.AddRange(containsMatch);

            return result.Distinct(new FuzzyMatchEqualityComparer<TCard>()).Take(maxResults);
        }

        private static double CalculateProbalityBasedOnContainsMatch(string nameToFind, TCard cardThatContainsString)
        {
            return (double) nameToFind.Length / cardThatContainsString.Name.Length;
        }
    }

    internal interface ICardNameLookupDataSource<TCard> where TCard : IHasName
    {
        IQueryable<TCard> AllDistinctCards { get; }
        Dictionary<long, TCard> HashLookup { get; }
    }

    internal class CardNameLookupDataSource<TCard> : ICardNameLookupDataSource<TCard> where TCard : IHasName
    {
        private IQueryable<TCard> _allCards;
        private Dictionary<long, TCard> _hashLookup;
        private Task _initTask;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly FNV1AHashAlgorithm64 FNV1AHashAlgorithm64 = new FNV1AHashAlgorithm64();

        public CardNameLookupDataSource(IQueryable<TCard> allCards)
        {
            if (allCards == null)
            {
                throw new ArgumentNullException(nameof(allCards));
            }
            
            var allDistinctCardsByCardName = allCards.Distinct(new CardNameEqualityComparer<TCard>());

#if HAVE_STRINGINTERN
            var orderCardsTask = Task.Run(() => { _allCards = allDistinctCardsByCardName.OrderBy(card => string.Intern(card.Name)).AsQueryable(); });
#else
            var orderCardsTask = Task.Run(() => { _allCards = allDistinctCardsByCardName.OrderBy(card => card.Name).AsQueryable(); });
#endif
            var createHashTableTask =
                Task.Run(
                    () =>
                    {
                        _hashLookup =
                            allDistinctCardsByCardName.ToDictionary(
                                card =>
                                {
#if HAVE_STRINGINTERN
                                    return
                                        FNV1AHashAlgorithm64.HashStringCaseInsensitive(string.Intern(string.Intern(card.Name).CreateValidUpperCaseKeyForString()));
#else
                                    return FNV1AHashAlgorithm64.HashStringCaseInsensitive(card.Name.CreateValidUpperCaseKeyForString());
#endif
                                }, card => card);
                    });

            _initTask = Task.Run(() =>
            {
                Task.WhenAll(orderCardsTask, createHashTableTask).Wait();
                _initTask = null;
            });
        }

        public IQueryable<TCard> AllDistinctCards
        {
            get
            {
                _initTask?.Wait();
                return _allCards;
            }           
        }

        public Dictionary<long, TCard> HashLookup
        {
            get
            {
                _initTask?.Wait();
                return _hashLookup;
            }
          
        }
    }
}