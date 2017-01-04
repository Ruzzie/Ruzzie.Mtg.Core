namespace Ruzzie.Mtg.Core
{
    public static class Formats
    {
        public static bool ContainsFormat(this Format format, Format formatToCheck)
        {
            if ((format & formatToCheck) != 0)
            {
                return true;
            }

            return false;
        }
    }
}