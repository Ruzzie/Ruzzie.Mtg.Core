using System;
using System.Linq;
using System.Text;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public class DeckExporterPlainText<TCard> : IDeckExporter<string, TCard> where TCard : IHasName
    {
        public string Export(in IDeckCards<TCard> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            var distinctMainBoard = cards.Mainboard.GroupBy(card => card.Card.Name, card => card.Count, (name, counts) => new {Name = name, Count = counts.Sum()});
            var distinctSideBoard = cards.Sideboard.GroupBy(card => card.Card.Name, card => card.Count, (name, counts) => new {Name = name, Count = counts.Sum()});

            StringBuilder textOutput = new StringBuilder();

            foreach (var item in distinctMainBoard)
            {
                textOutput.AppendLine($"{item.Count} {item.Name}");
            }

            if (cards.Sideboard.Any())
            {
                textOutput.AppendLine("Sideboard");
                foreach (var item in distinctSideBoard)
                {
                    textOutput.AppendLine($"{item.Count} {item.Name}");
                }
            }

            return textOutput.ToString();
        }
    }
}