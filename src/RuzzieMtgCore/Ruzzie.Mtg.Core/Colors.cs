using System;
using System.Collections.Concurrent;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Helper methods for the <see cref="Color"/> enum;
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// All colors
        /// </summary>
        public static readonly Color AllColors = Color.B | Color.G | Color.U | Color.W | Color.R;
        private static readonly ConcurrentDictionary<string,Color> EnumNameCache = new ConcurrentDictionary<string, Color>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Determines whether the specified colors to check contains color.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="colorsToCheck">The colors to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified colors to check contains color; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsColor(this Color colors,Color colorsToCheck)
        {
            if ((colors & colorsToCheck) != 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a <see cref="Color"/> enum with flags for the given input string.
        /// The expected format is uppercase colorcodes with no spaces ex.: U or UWG or BWGUR etc.
        /// </summary>
        /// <param name="colorsString">The input color codes string.</param>
        /// <returns></returns>
        public static Color From(string colorsString)
        {
            if (colorsString == null)
            {
                return Color.Colorless;
            }
#if HAVE_STRINGINTERN
            return EnumNameCache.GetOrAdd(string.Intern(colorsString), FromUncached);
#else
             return EnumNameCache.GetOrAdd(colorsString, FromUncached);
#endif
        }

        /// <summary>
        /// Creates a <see cref="Color"/> enum with flags for the given input string.
        /// The expected format is uppercase colorcodes with no spaces ex.: []{U} or []{U,W,G} or []{B,W,G,U,R} etc.
        /// </summary>
        /// <param name="colors">The input color codes array.</param>
        /// <returns></returns>
        public static Color From(string[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                return Color.Colorless;
            }

            Color color = Color.Colorless;

            for (int i = 0; i < colors.Length; i++)
            {
                string colorString = colors[i];
                if (colorString != "A" && colorString != "L" && colorString != "None")//For backwards compatibility reasons
                {
                    color |= From(colorString);                
                }
            }

            return color;
        }

        /// <summary>
        /// Determines whether [has more than one color].
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>
        ///   <c>true</c> if [has more than one color] [the specified color]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasMoreThanOneColor(this Color color)
        {
            return (color & (color - 1)) != 0;
        }

        /// <summary>
        /// Determines whether [has more than color count] [the specified count].
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        ///   <c>true</c> if [has more than color count] [the specified count]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasMoreThanColorCount(this Color colors, int count)
        {
            int numberOfColorsSet = GetNumberOfColors(colors);
            return numberOfColorsSet > count;
        }

        /// <summary>
        /// Gets the number of colors.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int GetNumberOfColors(this Color value)
        {
            int iCount = 0;

            //Loop the value while there are still bits
            while (value != 0)
            {
                //Remove the end bit
                value = value & (value - 1);

                //Increment the count
                iCount++;
            }

            //Return the count
            return iCount;
        }

        private static Color FromUncached(string colorsIdentityString)
        {
            int numberOfColors = colorsIdentityString?.Length ?? 0;
            Color colorIdentity = Color.Colorless;
            for (int i = 0; i < numberOfColors; i++)
            {
                Color enumResult;
                if (Enum.TryParse(colorsIdentityString?[i].ToString(), true, out enumResult))
                {
                    colorIdentity |= enumResult;
                }
            }
            return colorIdentity;
        }
    }
}