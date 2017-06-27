using System.Collections.Generic;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public class CardParseResult
    {
        public ParseResultCode ResultCode { get; set; }
        public List<CardNameAndCount> Cards { get; set; } = new List<CardNameAndCount>();
        public string Message { get; set; }
        public List<CardNameAndCount> Sideboard { get; set; } = new List<CardNameAndCount>();
    }
}