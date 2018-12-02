namespace Ruzzie.Mtg.Core.Data
{
    internal static class KnownSynonyms
    {
        public static string GetSynonym(string cardname)
        {
            if (string.IsNullOrWhiteSpace(cardname))
            {
                return null;
            }

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
                return  "Fa'adiyah Seer";
            }

            if (cardname.Contains("will o the wisp"))
            {
                return "Will-o'-the-Wisp";
            }

            if (cardname == "snapc")
            {
                return "Snapcaster Mage";
            }

            return null;
        }
    }
}