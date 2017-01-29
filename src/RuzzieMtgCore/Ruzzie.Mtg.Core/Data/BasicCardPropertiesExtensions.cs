namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Extension methods for <see cref="IBasicCardProperties"/>
    /// </summary>
    public static class BasicCardPropertiesExtensions
    {
        /// <summary>
        /// Determines whether [is land or basic land type].
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>
        ///   <c>true</c> if [is land or basic land type] [the specified information]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLandOrBasicLandType(this IBasicCardProperties info)
        {
            BasicType infoBasicType = (BasicType)info.BasicType;
            return infoBasicType.IsOnlyLandOrBasicLandType();
        }
    }
}